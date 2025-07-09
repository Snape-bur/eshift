// Sidebar toggle
document.getElementById("toggleSidebar").addEventListener("click", function () {
    document.getElementById("sidebar-wrapper").classList.toggle("collapsed");
});

// Dark mode toggle
const htmlTag = document.documentElement;
const themeSwitch = document.getElementById("themeSwitch");
const themeIcon = document.getElementById("themeIcon");

function applyTheme(theme) {
    htmlTag.setAttribute("data-bs-theme", theme);
    themeSwitch.checked = theme === "dark";
    themeIcon.className = theme === "dark" ? "bi bi-sun theme-toggle-icon" : "bi bi-moon theme-toggle-icon";
    localStorage.setItem("preferredTheme", theme);
}

// Load stored preference
const storedTheme = localStorage.getItem("preferredTheme") || "light";
applyTheme(storedTheme);

themeSwitch.addEventListener("change", () => {
    const newTheme = themeSwitch.checked ? "dark" : "light";
    applyTheme(newTheme);
});
