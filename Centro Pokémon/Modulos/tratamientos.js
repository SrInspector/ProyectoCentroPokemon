window.moduleInit = async function () {
    const content = document.getElementById("contenido");
    const role = getRole();
    const canEdit = role === "Administrador" || role === "Enfermero";
    content.innerHTML = `
        <section>
            <h2>Tratamientos</h2>
            ${canEdit ? `<form id="tratamientoForm">
                <select id="pokemonTratamiento" required></select>
                <input id="internamientoTratamiento" type="number" placeholder="Internamiento ID (opcional)">
                <input id="tipoTratamiento" placeholder="Tipo" required>
                <input id="dosisTratamiento" placeholder="Dosis" required>
                <input id="frecuenciaTratamiento" placeholder="Frecuencia" required>
                <input id="inicioTratamiento" type="datetime-local" required>
                <input id="finTratamiento" type="datetime-local" required>
                <label><input id="esCritico" type="checkbox"> Critico</label>
                <select id="estadoTratamiento">
                    <option value="Programado">Programado</option>
                    <option value="Activo">Activo</option>
                    <option value="Finalizado">Finalizado</option>
                    <option value="Cancelado">Cancelado</option>
                </select>
                <button type="submit">Registrar tratamiento</button>
            </form>` : `<p>Consulta habilitada segun permisos del rol.</p>`}
            <div id="tratamientosEstado"></div>
            <div id="tratamientosTabla"></div>
        </section>`;

    async function loadPokemonOptions() {
        if (!canEdit) return;
        const pokemons = await api("/Pokemon");
        pokemonTratamiento.innerHTML = pokemons.map((item) => `<option value="${item.id}">${escapeHtml(item.nombre)}</option>`).join("");
    }

    async function load() {
        try {
            const data = await api("/Tratamientos");
            tratamientosTabla.innerHTML = `
                <table border="1">
                    <tr><th>ID</th><th>Pokemon</th><th>Tipo</th><th>Dosis</th><th>Frecuencia</th><th>Inicio</th><th>Fin</th><th>Estado</th>${canEdit ? "<th>Acciones</th>" : ""}</tr>
                    ${data.map((item) => `
                        <tr>
                            <td>${item.id}</td>
                            <td>${escapeHtml(item.pokemon?.nombre || "-")}</td>
                            <td>${escapeHtml(item.tipo)}</td>
                            <td>${escapeHtml(item.dosis)}</td>
                            <td>${escapeHtml(item.frecuencia)}</td>
                            <td>${formatDate(item.fechaInicioUtc)}</td>
                            <td>${formatDate(item.fechaFinUtc)}</td>
                            <td>${escapeHtml(item.estado)}</td>
                            ${canEdit ? `<td>
                                <button data-state='${item.id}' data-next='Activo'>Activar</button>
                                <button data-state='${item.id}' data-next='Finalizado'>Finalizar</button>
                                <button data-state='${item.id}' data-next='Cancelado'>Cancelar</button>
                            </td>` : ""}
                        </tr>`).join("")}
                </table>`;

            document.querySelectorAll("[data-state]").forEach((button) => {
                button.addEventListener("click", async () => {
                    try {
                        await request(`/Tratamientos/${button.dataset.state}/estado`, { method: "PATCH", body: { estado: button.dataset.next } });
                        await load();
                    } catch (error) {
                        renderMessage("tratamientosEstado", error.message, "error");
                    }
                });
            });
        } catch (error) {
            renderMessage("tratamientosEstado", error.message, "error");
        }
    }

    if (canEdit) {
        await loadPokemonOptions();
        tratamientoForm.addEventListener("submit", async (event) => {
            event.preventDefault();
            const body = {
                pokemonId: Number(pokemonTratamiento.value),
                internamientoId: internamientoTratamiento.value ? Number(internamientoTratamiento.value) : null,
                tipo: tipoTratamiento.value.trim(),
                dosis: dosisTratamiento.value.trim(),
                frecuencia: frecuenciaTratamiento.value.trim(),
                fechaInicioUtc: new Date(inicioTratamiento.value).toISOString(),
                fechaFinUtc: new Date(finTratamiento.value).toISOString(),
                esCritico: esCritico.checked,
                estado: estadoTratamiento.value
            };
            try {
                await request("/Tratamientos", { method: "POST", body });
                event.target.reset();
                renderMessage("tratamientosEstado", "Tratamiento registrado correctamente.", "success");
                await load();
            } catch (error) {
                renderMessage("tratamientosEstado", error.message, "error");
            }
        });
    }

    await load();
};
