window.moduleInit = async function () {
    const content = document.getElementById("contenido");
    const role = getRole();
    const canCreateFactura = role === "Administrador" || role === "Enfermero";
    const canCreateServicio = role === "Administrador";
    content.innerHTML = `
        <section>
            <h2>Facturacion</h2>
            ${canCreateServicio ? `<form id="servicioForm">
                <h3>Nuevo servicio</h3>
                <input id="codigoServicio" placeholder="Codigo" required>
                <input id="nombreServicio" placeholder="Nombre" required>
                <input id="descripcionServicio" placeholder="Descripcion" required>
                <input id="costoBaseServicio" type="number" step="0.01" placeholder="Costo base" required>
                <label><input id="requiereAprobacion" type="checkbox"> Requiere aprobacion</label>
                <label><input id="esRecurrente" type="checkbox"> Es recurrente</label>
                <button type="submit">Crear servicio</button>
            </form>` : ""}
            ${canCreateFactura ? `<form id="facturaForm">
                <h3>Nueva factura</h3>
                <select id="entrenadorFactura" required></select>
                <select id="servicioFactura" required></select>
                <input id="cantidadFactura" type="number" min="1" value="1" required>
                <button type="submit">Generar factura</button>
            </form>` : ""}
            <div id="facturacionEstado"></div>
            <div id="serviciosTabla"></div>
            <div id="facturasTabla"></div>
        </section>`;

    async function loadData() {
        const [servicios, facturas] = await Promise.all([api("/Facturacion/servicios"), api("/Facturacion/facturas")]);
        serviciosTabla.innerHTML = `<h3>Servicios</h3><table border="1"><tr><th>Codigo</th><th>Nombre</th><th>Costo</th><th>Recurrente</th></tr>${servicios.map((item) => `<tr><td>${escapeHtml(item.codigo)}</td><td>${escapeHtml(item.nombre)}</td><td>${item.costoBase}</td><td>${item.esRecurrente ? "Si" : "No"}</td></tr>`).join("")}</table>`;
        facturasTabla.innerHTML = `<h3>Facturas</h3><table border="1"><tr><th>ID</th><th>Referencia</th><th>Entrenador</th><th>Fecha</th><th>Total</th><th>Estado</th><th>Comprobante</th></tr>${facturas.map((item) => `<tr><td>${item.id}</td><td>${escapeHtml(item.referencia)}</td><td>${escapeHtml(item.entrenador?.nombre || "-")}</td><td>${formatDate(item.fechaEmisionUtc)}</td><td>${item.total}</td><td>${escapeHtml(item.estado)}</td><td><button data-comprobante='${item.id}'>Descargar</button></td></tr>`).join("")}</table>`;

        if (canCreateFactura) {
            const entrenadores = await api("/Entrenadores");
            entrenadorFactura.innerHTML = entrenadores.map((item) => `<option value="${item.id}">${escapeHtml(item.nombre)}</option>`).join("");
            servicioFactura.innerHTML = servicios.map((item) => `<option value="${item.id}">${escapeHtml(item.nombre)}</option>`).join("");
        }

        document.querySelectorAll("[data-comprobante]").forEach((button) => {
            button.addEventListener("click", async () => {
                try {
                    const blob = await request(`/Facturacion/facturas/${button.dataset.comprobante}/comprobante`, { responseType: "blob" });
                    const url = URL.createObjectURL(blob);
                    const link = document.createElement("a");
                    link.href = url;
                    link.download = `comprobante-${button.dataset.comprobante}.pdf`;
                    link.click();
                    URL.revokeObjectURL(url);
                } catch (error) {
                    renderMessage("facturacionEstado", error.message, "error");
                }
            });
        });
    }

    if (canCreateServicio) {
        servicioForm.addEventListener("submit", async (event) => {
            event.preventDefault();
            const body = {
                codigo: codigoServicio.value.trim(),
                nombre: nombreServicio.value.trim(),
                descripcion: descripcionServicio.value.trim(),
                costoBase: Number(costoBaseServicio.value),
                requiereAprobacionAdministrador: requiereAprobacion.checked,
                esRecurrente: esRecurrente.checked
            };
            try {
                await request("/Facturacion/servicios", { method: "POST", body });
                event.target.reset();
                renderMessage("facturacionEstado", "Servicio creado correctamente.", "success");
                await loadData();
            } catch (error) {
                renderMessage("facturacionEstado", error.message, "error");
            }
        });
    }

    if (canCreateFactura) {
        facturaForm.addEventListener("submit", async (event) => {
            event.preventDefault();
            const servicioId = Number(servicioFactura.value);
            const cantidad = Number(cantidadFactura.value);
            const descripcion = servicioFactura.options[servicioFactura.selectedIndex]?.textContent || "Servicio";
            const body = {
                entrenadorId: Number(entrenadorFactura.value),
                detalles: [{ servicioClinicoId: servicioId, cantidad, descripcion }]
            };
            try {
                await request("/Facturacion/facturas", { method: "POST", body });
                event.target.reset();
                renderMessage("facturacionEstado", "Factura creada correctamente.", "success");
                await loadData();
            } catch (error) {
                renderMessage("facturacionEstado", error.message, "error");
            }
        });
    }

    try {
        await loadData();
    } catch (error) {
        renderMessage("facturacionEstado", error.message, "error");
    }
};
