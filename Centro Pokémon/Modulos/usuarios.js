window.moduleInit = async function () {
    if (!canAccess("usuarios")) {
        renderMessage("contenido", "No tienes acceso a este modulo.", "error");
        return;
    }

    const content = document.getElementById("contenido");
    content.innerHTML = `
        <section>
            <h2>Usuarios</h2>
            <p>El backend actual permite crear usuarios con roles, pero no expone un listado completo de usuarios.</p>
            <form id="userForm">
                <input id="nombreCompleto" placeholder="Nombre completo" required>
                <input id="correo" type="email" placeholder="Correo" required>
                <input id="password" type="password" placeholder="Contrasena" required>
                <select id="rol">
                    <option value="Administrador">Administrador</option>
                    <option value="Enfermero">Enfermero</option>
                    <option value="Entrenador">Entrenador</option>
                </select>
                <select id="entrenadorId">
                    <option value="">Sin entrenador asociado</option>
                </select>
                <button type="submit">Crear usuario</button>
            </form>
            <div id="usuariosEstado"></div>
        </section>`;

    try {
        const entrenadores = await api("/Entrenadores");
        const select = document.getElementById("entrenadorId");
        entrenadores.forEach((item) => {
            const option = document.createElement("option");
            option.value = item.id;
            option.textContent = `${item.nombre} (${item.identificacion})`;
            select.appendChild(option);
        });
    } catch {
        renderMessage("usuariosEstado", "No fue posible cargar entrenadores para asociar usuarios entrenador.", "error");
    }

    document.getElementById("rol").addEventListener("change", (event) => {
        document.getElementById("entrenadorId").disabled = event.target.value !== "Entrenador";
    });
    document.getElementById("rol").dispatchEvent(new Event("change"));

    document.getElementById("userForm").addEventListener("submit", async (event) => {
        event.preventDefault();
        const body = {
            nombreCompleto: document.getElementById("nombreCompleto").value.trim(),
            correo: document.getElementById("correo").value.trim(),
            password: document.getElementById("password").value,
            rol: document.getElementById("rol").value,
            entrenadorId: document.getElementById("entrenadorId").value ? Number(document.getElementById("entrenadorId").value) : null
        };

        try {
            const response = await request("/Auth/usuarios", { method: "POST", body });
            renderMessage("usuariosEstado", `Usuario creado: ${escapeHtml(response.nombreCompleto)} (${escapeHtml(response.rol)})`, "success");
            event.target.reset();
            document.getElementById("rol").dispatchEvent(new Event("change"));
        } catch (error) {
            renderMessage("usuariosEstado", error.message, "error");
        }
    });
};
