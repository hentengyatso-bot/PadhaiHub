// Shows the chosen image straight away and blocks files that are too big or the wrong type.
document.querySelectorAll('input[type="file"]').forEach(function (input) {
    input.addEventListener("change", function () {
        const file = this.files[0];
        const preview = document.getElementById(this.dataset.preview || "photoPreview");
        if (!file) return;

        const allowed = ["image/jpeg", "image/png", "image/webp"];
        if (!allowed.includes(file.type)) {
            alert("Please choose a JPG, PNG or WEBP image.");
            this.value = "";
            return;
        }
        if (file.size > 2 * 1024 * 1024) {
            alert("The image must be smaller than 2 MB.");
            this.value = "";
            return;
        }
        if (preview) {
            preview.src = URL.createObjectURL(file);
        }
    });
});