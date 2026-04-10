window.moduleInit = async function () {
    if (!canAccess("reportes")) {
        renderMessage("contenido", "No tienes acceso a este modulo.", "error");
        return;
    }

    const content = document.getElementById("contenido");
    content.innerHTML = `
        <section>
            <h2>Reportes y Auditoria</h2>
            <form id="reporteForm">
                <input id="fechaInicioReporte" type="date" required>
                <input id="fechaFinReporte" type="date" required>
                <button type="submit">Cargar reportes</button>
                <button type="button" id="auditoriaBtn">Cargar auditoria</button>
                <button type="button" id="exportTopBtn">Exportar top pokemon</button>
            </form>
            <div id="reportesEstado"></div>
            <div id="reportesContenido"></div>
        </section>`;

    function getFiltro() {
        return {
            fechaInicioUtc: new Date(fechaInicioReporte.value).toISOString(),
            fechaFinUtc: new Date(fechaFinReporte.value).toISOString()
        };
    }

    reporteForm.addEventListener("submit", async (event) => {
        event.preventDefault();
        try {
            const data = await request("/Reportes", { method: "POST", body: getFiltro() });
            reportesContenido.innerHTML = `<pre>${escapeHtml(JSON.stringify(data, null, 2))}</pre>`;
        } catch (error) {
            renderMessage("reportesEstado", error.message, "error");
        }
    });

    auditoriaBtn.addEventListener("click", async () => {
        try {
            const data = await request("/Reportes/auditoria");
            reportesContenido.innerHTML = `<pre>${escapeHtml(JSON.stringify(data, null, 2))}</pre>`;
        } catch (error) {
            renderMessage("reportesEstado", error.message, "error");
        }
    });

    exportTopBtn.addEventListener("click", async () => {
        try {
            const blob = await request("/Reportes/top-pokemon/exportar", { method: "POST", body: getFiltro(), responseType: "blob" });
            const url = URL.createObjectURL(blob);
            const link = document.createElement("a");
            link.href = url;
            link.download = "reporte-top-pokemon.csv";
            link.click();
            URL.revokeObjectURL(url);
        } catch (error) {
            renderMessage("reportesEstado", error.message, "error");
        }
    });
};
