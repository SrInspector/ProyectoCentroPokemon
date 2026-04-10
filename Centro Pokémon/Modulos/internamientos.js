window.moduleInit = async function () {
    const content = document.getElementById("contenido");
    const role = getRole();
    const canCreate = role === "Administrador" || role === "Enfermero";
    const canAlta = role === "Administrador";
    content.innerHTML = `
        <section>
            <h2>Internamientos</h2>
            ${canCreate ? `<form id="internamientoForm">
                <select id="pokemonIdInternamiento" required></select>
                <input id="fechaIngresoUtc" type="datetime-local" required>
                <input id="motivoInternamiento" placeholder="Motivo" required>
                <input id="areaAsignada" placeholder="Area asignada" required>
                <select id="estadoInternamiento">
                    <option value="Activo">Activo</option>
                    <option value="Observacion">Observacion</option>
                </select>
                <button type="submit">Registrar internamiento</button>
            </form>` : `<p>Consulta habilitada segun permisos del rol.</p>`}
            <div id="internamientosEstado"></div>
            <div id="internamientosTabla"></div>
        </section>`;

    async function loadPokemonOptions() {
        if (!canCreate) return;
        const pokemons = await api("/Pokemon");
        pokemonIdInternamiento.innerHTML = pokemons.map((item) => `<option value="${item.id}">${escapeHtml(item.nombre)}</option>`).join("");
    }

    async function load() {
        try {
            const data = await api("/Internamientos");
            internamientosTabla.innerHTML = `
                <table border="1">
                    <tr><th>Codigo</th><th>Pokemon</th><th>Ingreso</th><th>Area</th><th>Estado</th>${canAlta ? "<th>Alta</th>" : ""}</tr>
                    ${data.map((item) => `
                        <tr>
                            <td>${escapeHtml(item.codigo)}</td>
                            <td>${escapeHtml(item.pokemon?.nombre || "-")}</td>
                            <td>${formatDate(item.fechaIngresoUtc)}</td>
                            <td>${escapeHtml(item.areaAsignada)}</td>
                            <td>${escapeHtml(item.estado)}</td>
                            ${canAlta ? `<td>${item.estado !== "AltaMedica" ? `<button data-alta='${item.id}'>Dar alta</button>` : "-"}</td>` : ""}
                        </tr>`).join("")}
                </table>`;

            document.querySelectorAll("[data-alta]").forEach((button) => {
                button.addEventListener("click", async () => {
                    const observacion = prompt("Observacion de alta medica:", "Alta autorizada") || "Alta autorizada";
                    try {
                        await request(`/Internamientos/${button.dataset.alta}/alta`, { method: "PATCH", body: { observacion } });
                        await load();
                    } catch (error) {
                        renderMessage("internamientosEstado", error.message, "error");
                    }
                });
            });
        } catch (error) {
            renderMessage("internamientosEstado", error.message, "error");
        }
    }

    if (canCreate) {
        await loadPokemonOptions();
        internamientoForm.addEventListener("submit", async (event) => {
            event.preventDefault();
            const body = {
                pokemonId: Number(pokemonIdInternamiento.value),
                fechaIngresoUtc: new Date(fechaIngresoUtc.value).toISOString(),
                motivo: motivoInternamiento.value.trim(),
                areaAsignada: areaAsignada.value.trim(),
                estado: estadoInternamiento.value
            };
            try {
                await request("/Internamientos", { method: "POST", body });
                event.target.reset();
                renderMessage("internamientosEstado", "Internamiento registrado correctamente.", "success");
                await load();
            } catch (error) {
                renderMessage("internamientosEstado", error.message, "error");
            }
        });
    }

    await load();
};
