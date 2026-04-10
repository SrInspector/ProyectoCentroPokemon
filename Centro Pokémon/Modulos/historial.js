window.moduleInit = async function () {
    const content = document.getElementById("contenido");
    content.innerHTML = `
        <section>
            <h2>Historial Clinico</h2>
            <div>
                <select id="pokemonHistorial"></select>
                <button id="cargarHistorialBtn">Consultar</button>
                <button id="pdfHistorialBtn">Expediente PDF</button>
                <button id="csvHistorialBtn">Expediente CSV</button>
            </div>
            <div id="historialEstado"></div>
            <div id="historialTabla"></div>
        </section>`;

    const pokemons = await api("/Pokemon");
    pokemonHistorial.innerHTML = pokemons.map((item) => `<option value="${item.id}">${escapeHtml(item.nombre)}</option>`).join("");

    async function loadHistorial() {
        try {
            const data = await request(`/Historial/pokemon/${pokemonHistorial.value}`);
            historialTabla.innerHTML = `<pre>${escapeHtml(JSON.stringify(data, null, 2))}</pre>`;
        } catch (error) {
            renderMessage("historialEstado", error.message, "error");
        }
    }

    async function exportFile(format) {
        try {
            const blob = await request(`/Historial/pokemon/${pokemonHistorial.value}/expediente?formato=${format}`, { responseType: "blob" });
            const url = URL.createObjectURL(blob);
            const link = document.createElement("a");
            link.href = url;
            link.download = `expediente-${pokemonHistorial.value}.${format === "pdf" ? "pdf" : "csv"}`;
            link.click();
            URL.revokeObjectURL(url);
        } catch (error) {
            renderMessage("historialEstado", error.message, "error");
        }
    }

    cargarHistorialBtn.addEventListener("click", loadHistorial);
    pdfHistorialBtn.addEventListener("click", () => exportFile("pdf"));
    csvHistorialBtn.addEventListener("click", () => exportFile("csv"));
};
