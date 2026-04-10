window.moduleInit = async function () {
    const content = document.getElementById("contenido");
    const role = getRole();
    const canEdit = role === "Administrador" || role === "Enfermero";
    const canDelete = role === "Administrador";
    content.innerHTML = `
        <section>
            <h2>Citas</h2>
            ${canEdit ? `<form id="citaForm">
                <input id="citaId" type="hidden">
                <input id="fechaProgramadaUtc" type="datetime-local" required>
                <input id="motivo" placeholder="Motivo" required>
                <select id="entrenadorIdCita" required></select>
                <select id="pokemonIdCita" required></select>
                <input id="usuarioAsignadoId" type="number" placeholder="Usuario asignado (opcional)">
                <button type="submit">Guardar cita</button>
            </form>` : `<p>Consulta habilitada segun permisos del rol.</p>`}
            <div id="citasEstado"></div>
            <div id="citasTabla"></div>
        </section>`;

    async function loadOptions() {
        if (!canEdit) return;
        const [entrenadores, pokemons] = await Promise.all([api("/Entrenadores"), api("/Pokemon")]);
        entrenadorIdCita.innerHTML = entrenadores.map((item) => `<option value="${item.id}">${escapeHtml(item.nombre)}</option>`).join("");
        pokemonIdCita.innerHTML = pokemons.map((item) => `<option value="${item.id}">${escapeHtml(item.nombre)}</option>`).join("");
    }

    async function load() {
        try {
            const data = await api("/Citas");
            citasTabla.innerHTML = `
                <table border="1">
                    <tr><th>ID</th><th>Fecha</th><th>Motivo</th><th>Estado</th><th>Pokemon</th><th>Entrenador</th>${canEdit || canDelete ? "<th>Acciones</th>" : ""}</tr>
                    ${data.map((item) => `
                        <tr>
                            <td>${item.id}</td>
                            <td>${formatDate(item.fechaProgramadaUtc)}</td>
                            <td>${escapeHtml(item.motivo)}</td>
                            <td>${escapeHtml(item.estado)}</td>
                            <td>${escapeHtml(item.pokemon?.nombre || "-")}</td>
                            <td>${escapeHtml(item.entrenador?.nombre || "-")}</td>
                            ${canEdit || canDelete ? `<td>
                                ${canEdit ? `<button data-edit='${JSON.stringify(item)}'>Editar</button><button data-state='${item.id}' data-next='Confirmada'>Confirmar</button>` : ""}
                                ${canDelete ? `<button data-delete='${item.id}'>Eliminar</button>` : ""}
                            </td>` : ""}
                        </tr>`).join("")}
                </table>`;

            document.querySelectorAll("[data-edit]").forEach((button) => {
                button.addEventListener("click", () => {
                    const item = JSON.parse(button.dataset.edit);
                    citaId.value = item.id;
                    fechaProgramadaUtc.value = item.fechaProgramadaUtc?.slice(0, 16) || "";
                    motivo.value = item.motivo;
                    entrenadorIdCita.value = item.entrenadorId;
                    pokemonIdCita.value = item.pokemonId;
                    usuarioAsignadoId.value = item.usuarioAsignadoId || "";
                });
            });

            document.querySelectorAll("[data-state]").forEach((button) => {
                button.addEventListener("click", async () => {
                    try {
                        await request(`/Citas/${button.dataset.state}/estado`, { method: "PATCH", body: { estado: button.dataset.next } });
                        await load();
                    } catch (error) {
                        renderMessage("citasEstado", error.message, "error");
                    }
                });
            });

            document.querySelectorAll("[data-delete]").forEach((button) => {
                button.addEventListener("click", async () => {
                    if (!confirm("Eliminar cita?")) return;
                    try {
                        await request(`/Citas/${button.dataset.delete}`, { method: "DELETE" });
                        await load();
                    } catch (error) {
                        renderMessage("citasEstado", error.message, "error");
                    }
                });
            });
        } catch (error) {
            renderMessage("citasEstado", error.message, "error");
        }
    }

    if (canEdit) {
        await loadOptions();
        citaForm.addEventListener("submit", async (event) => {
            event.preventDefault();
            const id = citaId.value;
            const body = {
                fechaProgramadaUtc: new Date(fechaProgramadaUtc.value).toISOString(),
                motivo: motivo.value.trim(),
                entrenadorId: Number(entrenadorIdCita.value),
                pokemonId: Number(pokemonIdCita.value),
                usuarioAsignadoId: usuarioAsignadoId.value ? Number(usuarioAsignadoId.value) : null
            };
            try {
                await request(id ? `/Citas/${id}` : "/Citas", { method: id ? "PUT" : "POST", body });
                event.target.reset();
                citaId.value = "";
                renderMessage("citasEstado", "Cita guardada correctamente.", "success");
                await load();
            } catch (error) {
                renderMessage("citasEstado", error.message, "error");
            }
        });
    }

    await load();
};
