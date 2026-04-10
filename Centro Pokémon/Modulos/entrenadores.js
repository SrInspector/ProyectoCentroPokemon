window.moduleInit = async function () {
    const content = document.getElementById("contenido");
    const canEdit = getRole() === "Administrador";
    content.innerHTML = `
        <section>
            <h2>Entrenadores</h2>
            ${canEdit ? `<form id="trainerForm">
                <input id="trainerId" type="hidden">
                <input id="identificacion" placeholder="Identificacion" required>
                <input id="nombre" placeholder="Nombre" required>
                <input id="email" type="email" placeholder="Email" required>
                <input id="telefono" placeholder="Telefono" required>
                <button type="submit">Guardar</button>
                <button type="button" id="cancelTrainer">Cancelar</button>
            </form>` : `<p>Consulta habilitada para este rol.</p>`}
            <div id="entrenadoresEstado"></div>
            <div id="entrenadoresTabla"></div>
        </section>`;

    async function load() {
        try {
            const data = await api("/Entrenadores");
            document.getElementById("entrenadoresTabla").innerHTML = `
                <table border="1">
                    <tr><th>ID</th><th>Identificacion</th><th>Nombre</th><th>Email</th><th>Telefono</th>${canEdit ? "<th>Acciones</th>" : ""}</tr>
                    ${data.map((item) => `
                        <tr>
                            <td>${item.id}</td>
                            <td>${escapeHtml(item.identificacion)}</td>
                            <td>${escapeHtml(item.nombre)}</td>
                            <td>${escapeHtml(item.email)}</td>
                            <td>${escapeHtml(item.telefono)}</td>
                            ${canEdit ? `<td>
                                <button data-edit='${JSON.stringify(item)}'>Editar</button>
                                <button data-delete='${item.id}'>Eliminar</button>
                            </td>` : ""}
                        </tr>`).join("")}
                </table>`;

            document.querySelectorAll("[data-edit]").forEach((button) => {
                button.addEventListener("click", () => {
                    const item = JSON.parse(button.dataset.edit);
                    trainerId.value = item.id;
                    identificacion.value = item.identificacion;
                    nombre.value = item.nombre;
                    email.value = item.email;
                    telefono.value = item.telefono;
                });
            });

            document.querySelectorAll("[data-delete]").forEach((button) => {
                button.addEventListener("click", async () => {
                    if (!confirm("Eliminar entrenador?")) return;
                    try {
                        await request(`/Entrenadores/${button.dataset.delete}`, { method: "DELETE" });
                        await load();
                    } catch (error) {
                        renderMessage("entrenadoresEstado", error.message, "error");
                    }
                });
            });
        } catch (error) {
            renderMessage("entrenadoresEstado", error.message, "error");
        }
    }

    if (canEdit) {
        document.getElementById("trainerForm").addEventListener("submit", async (event) => {
            event.preventDefault();
            const id = document.getElementById("trainerId").value;
            const body = {
                identificacion: identificacion.value.trim(),
                nombre: nombre.value.trim(),
                email: email.value.trim(),
                telefono: telefono.value.trim()
            };

            try {
                await request(id ? `/Entrenadores/${id}` : "/Entrenadores", { method: id ? "PUT" : "POST", body });
                event.target.reset();
                trainerId.value = "";
                renderMessage("entrenadoresEstado", "Entrenador guardado correctamente.", "success");
                await load();
            } catch (error) {
                renderMessage("entrenadoresEstado", error.message, "error");
            }
        });

        document.getElementById("cancelTrainer").addEventListener("click", () => {
            document.getElementById("trainerForm").reset();
            trainerId.value = "";
        });
    }

    await load();
};
