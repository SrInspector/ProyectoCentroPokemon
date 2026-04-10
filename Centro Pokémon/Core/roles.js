const roleModules = {
    Administrador: ["usuarios", "entrenadores", "pokemon", "citas", "internamientos", "tratamientos", "facturacion", "historial", "reportes"],
    Enfermero: ["pokemon", "citas", "internamientos", "tratamientos", "facturacion", "historial"],
    Entrenador: ["pokemon", "citas", "tratamientos", "facturacion", "historial"]
};

function getRole() {
    return localStorage.getItem("rol") || getSession()?.rol || "";
}

function canAccess(moduleName) {
    const allowed = roleModules[getRole()] || [];
    return allowed.includes(moduleName);
}

function applyRoleVisibility() {
    const role = getRole();
    document.querySelectorAll("[data-module]").forEach((item) => {
        item.style.display = canAccess(item.dataset.module) ? "inline-flex" : "none";
    });

    const roleLabel = document.getElementById("roleLabel");
    const userLabel = document.getElementById("userLabel");
    const session = getSession();

    if (roleLabel) roleLabel.textContent = role || "Sin rol";
    if (userLabel) userLabel.textContent = session?.nombreCompleto || session?.correo || "Usuario";
}
