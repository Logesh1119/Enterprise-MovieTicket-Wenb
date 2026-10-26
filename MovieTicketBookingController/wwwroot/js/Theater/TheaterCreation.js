const input = document.getElementById("imageInput");
const preview = document.getElementById("previewContainer");

if (input && preview) {
    input.addEventListener("change", function () {
        preview.innerHTML = "";
        [...this.files].forEach(file => {
            const reader = new FileReader();
            reader.onload = function (e) {
                preview.innerHTML += `
                        <div class="col-6 col-md-3">
                            <div class="rounded-3 overflow-hidden border border-secondary-subtle bg-dark position-relative shadow-sm" style="height:140px;">
                                <img src="${e.target.result}" class="w-100 h-100" style="object-fit:cover;">
                            </div>
                        </div>
                        `;
            }
            reader.readAsDataURL(file);
        });
    });
}