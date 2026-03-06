window.NT = (function () {
    function applyTheme(theme) {
        const body = document.body;
        body.classList.toggle("nt-light", theme === "light");
        localStorage.setItem("nt_theme", theme);
    }

    function initTheme() {
        const saved = localStorage.getItem("nt_theme");
        applyTheme(saved || "dark");
    }

    function toggleTheme() {
        const isLight = document.body.classList.contains("nt-light");
        applyTheme(isLight ? "dark" : "light");
        toast(isLight ? "Tema oscuro activado" : "Tema claro activado");
    }

    function toggleSidebar() {
        document.body.classList.toggle("nt-sidebar-open");
    }

    function toast(msg) {
        try {
            const el = document.getElementById("ntToast");
            const body = document.getElementById("ntToastBody");
            if (!el || !body) return;
            body.textContent = msg || "Listo.";
            const t = bootstrap.Toast.getOrCreateInstance(el, { delay: 2200 });
            t.show();
        } catch { }
    }

    document.addEventListener("DOMContentLoaded", initTheme);

    return { toggleTheme, toggleSidebar, toast };
})();