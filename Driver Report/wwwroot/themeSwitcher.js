window.themeSwitcher = {
    setTheme: function (theme) {
        document.documentElement.setAttribute('data-bs-theme', theme);
        localStorage.setItem('driverReportTheme', theme);
    },
    getTheme: function () {
        return localStorage.getItem('driverReportTheme') || 'light';
    },

    toggleTheme: function () {
        var currentTheme = this.getTheme();
        var newTheme = currentTheme === 'dark' ? 'light' : 'dark';
        this.setTheme(newTheme);
        return newTheme;
    }
};