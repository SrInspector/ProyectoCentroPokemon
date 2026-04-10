window.moduleInit = async function () {
    const content = document.getElementById("contenido");
    const canEdit = getRole() === "Administrador";
    content.innerHTML = `
        <section>
            <h2>Pokemon</h2>
            ${canEdit ? `<form id="pokemonForm">
                <input id="pokemonId" type="hidden">
                <input id="identificadorUnico" placeholder="Identificador unico" required>
                <input id="nombrePokemon" placeholder="Nombre" required>
                <input id="especie" placeholder="Especie" required>
                <input id="nivel" type="number" min="1" max="100" placeholder="Nivel" required>
                <input id="tipoPrimario" placeholder="Tipo primario" required>
                <input id="tipoSecundario" placeholder="Tipo secundario">
                <select id="estadoSalud">
                    <option value="Estable">Estable</option>
                    <option value="EnTratamiento">EnTratamiento</option>
                    <option value="Hospitalizado">Hospitalizado</option>
                    <option value="Recuperado">Recuperado</option>
                    <option value="Critico">Critico</option>
                </select>
                <select id="entrenadorPokemon" required></select>
                <button type="submit">Guardar</button>
            </form>` : `<p>Consulta habilitada segun permisos del rol.</p>`}
            <div id="pokemonEstado"></div>
            <div id="pokemonTabla"></div>
        </section>`;

    async function loadTrainerOptions() {
        if (!canEdit) return;
        const trainers = await api("/Entrenadores");
        entrenadorPokemon.innerHTML = trainers.map((item) => `<option value="${item.id}">${escapeHtml(item.nombre)} (${escapeHtml(item.identificacion)})</option>`).join("");
    }

    async function load() {
        try {
            const data = await api("/Pokemon");
            pokemonTabla.innerHTML = `
                <table border="1">
                    <tr><th>ID</th><th>Nombre</th><th>Especie</th><th>Nivel</th><th>Estado</th><th>Entrenador</th>${canEdit ? "<th>Acciones</th>" : ""}</tr>
                    ${data.map((item) => `
                        <tr>
                            <td>${item.id}</td>
                            <td>${escapeHtml(item.nombre)}</td>
                            <td>${escapeHtml(item.especie)}</td>
                            <td>${item.nivel}</td>
                            <td>${escapeHtml(item.estadoSalud)}</td>
                            <td>${escapeHtml(item.entrenador?.nombre || "-")}</td>
                            ${canEdit ? `<td>
                                <button data-edit='${JSON.stringify(item)}'>Editar</button>
                                <button data-delete='${item.id}'>Eliminar</button>
                            </td>` : ""}
                        </tr>`).join("")}
                </table>`;

            document.querySelectorAll("[data-edit]").forEach((button) => {
                button.addEventListener("click", () => {
                    const item = JSON.parse(button.dataset.edit);
                    pokemonId.value = item.id;
                    identificadorUnico.value = item.identificadorUnico;
                    nombrePokemon.value = item.nombre;
                    especie.value = item.especie;
                    nivel.value = item.nivel;
                    tipoPrimario.value = item.tipoPrimario;
                    tipoSecundario.value = item.tipoSecundario || "";
                    estadoSalud.value = item.estadoSalud;
                    entrenadorPokemon.value = item.entrenadorId;
                });
            });

            document.querySelectorAll("[data-delete]").forEach((button) => {
                button.addEventListener("click", async () => {
                    if (!confirm("Eliminar pokemon?")) return;
                    try {
                        await request(`/Pokemon/${button.dataset.delete}`, { method: "DELETE" });
                        await load();
                    } catch (error) {
                        renderMessage("pokemonEstado", error.message, "error");
                    }
                });
            });
        } catch (error) {
            renderMessage("pokemonEstado", error.message, "error");
        }
    }

    if (canEdit) {
        await loadTrainerOptions();
        pokemonForm.addEventListener("submit", async (event) => {
            event.preventDefault();
            const id = pokemonId.value;
            const body = {
                identificadorUnico: identificadorUnico.value.trim(),
                nombre: nombrePokemon.value.trim(),
                especie: especie.value.trim(),
                nivel: Number(nivel.value),
                tipoPrimario: tipoPrimario.value.trim(),
                tipoSecundario: tipoSecundario.value.trim() || null,
                estadoSalud: estadoSalud.value,
                entrenadorId: Number(entrenadorPokemon.value)
            };

            try {
                await request(id ? `/Pokemon/${id}` : "/Pokemon", { method: id ? "PUT" : "POST", body });
                event.target.reset();
                pokemonId.value = "";
                renderMessage("pokemonEstado", "Pokemon guardado correctamente.", "success");
                await load();
            } catch (error) {
                renderMessage("pokemonEstado", error.message, "error");
            }
        });
    }

    await load();
};
