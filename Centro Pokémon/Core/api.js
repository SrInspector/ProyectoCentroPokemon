const API = "http://localhost:5011/api";

function getToken() {
    return localStorage.getItem("token") || "";
}

function getHeaders(extraHeaders = {}) {
    const headers = {
        Accept: "application/json",
        ...extraHeaders
    };

    const token = getToken();
    if (token) {
        headers.Authorization = `Bearer ${token}`;
    }

    return headers;
}

async function request(path, options = {}) {
    const { method = "GET", body, responseType = "json", headers = {} } = options;
    const config = { method, headers: getHeaders(headers) };

    if (body !== undefined) {
        config.body = JSON.stringify(body);
        config.headers["Content-Type"] = "application/json";
    }

    const response = await fetch(`${API}${path}`, config);

    if (response.status === 401) {
        logout();
        throw new Error("Tu sesion vencio. Inicia sesion nuevamente.");
    }

    if (!response.ok) {
        let message = "No fue posible completar la solicitud.";
        try {
            const error = await response.json();
            message = error.mensaje || error.message || message;
        } catch {}

        throw new Error(message);
    }

    if (responseType === "blob") {
        return response.blob();
    }

    if (response.status === 204) {
        return null;
    }

    return response.json();
}

async function api(resource, method = "GET", body = undefined) {
    const normalized = resource.startsWith("/") ? resource : `/${resource}`;
    return request(normalized, { method, body });
}

function formatDate(value) {
    if (!value) return "-";
    return new Date(value).toLocaleString("es-CR");
}

function renderMessage(containerId, message, type = "info") {
    const container = document.getElementById(containerId);
    if (!container) return;
    container.innerHTML = `<div class="message message-${type}">${message}</div>`;
}

function escapeHtml(value) {
    return String(value ?? "")
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;")
        .replace(/\"/g, "&quot;")
        .replace(/'/g, "&#39;");
}
