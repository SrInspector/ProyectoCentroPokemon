function getSession() {
    const raw = localStorage.getItem("auth");
    return raw ? JSON.parse(raw) : null;
}

function saveSession(response) {
    localStorage.setItem("token", response.token || "");
    localStorage.setItem("auth", JSON.stringify(response));
    localStorage.setItem("rol", response.rol || "");
}

async function login() {
    const correo = document.getElementById("email")?.value?.trim();
    const password = document.getElementById("password")?.value || "";

    if (!correo || !password) {
        alert("Debes completar correo y contrasena.");
        return;
    }

    try {
        const data = await request("/Auth/login", {
            method: "POST",
            body: { correo, password }
        });

        saveSession(data);
        window.location.href = "dashboard.html";
    } catch (error) {
        alert(error.message || "No fue posible iniciar sesion.");
    }
}

function logout() {
    localStorage.removeItem("token");
    localStorage.removeItem("auth");
    localStorage.removeItem("rol");
    window.location.href = "index.html";
}

function requireAuth() {
    if (!getToken()) {
        window.location.href = "index.html";
    }
}
