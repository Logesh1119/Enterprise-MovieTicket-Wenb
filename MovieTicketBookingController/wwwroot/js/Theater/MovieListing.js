/* ================================================================
   MOVIE LISTING — Drawer JS
   Same architecture as TheaterListing.js
================================================================ */

(function () {
    'use strict';

    /* ── DOM references ────────────────────────────────────────── */
    var overlay = document.getElementById('movieDrawerOverlay');
    var drawer  = document.getElementById('movieDrawer');
    var form    = document.getElementById('movieDrawerForm');
    var title   = document.getElementById('movieDrawerTitle');
    var saveBtn = document.getElementById('btnMovieDrawerSave');
    var saveLbl = document.getElementById('btnMovieDrawerSaveLabel');
    var errDiv  = document.getElementById('movieDrawerErrorSummary');
    var sucDiv  = document.getElementById('movieDrawerSuccessBanner');
    var srchInp = document.getElementById('movieSearchInput');

    if (!drawer) return; // guard

    /* ── Open / Close helpers ──────────────────────────────────── */
    function openDrawer() {
        drawer.classList.add('is-open');
        if (overlay) overlay.classList.add('is-open');
        document.body.style.overflow = 'hidden';
    }

    function closeDrawer() {
        drawer.classList.remove('is-open');
        if (overlay) overlay.classList.remove('is-open');
        document.body.style.overflow = '';
        if (errDiv)  { errDiv.style.display = 'none'; errDiv.innerHTML = ''; }
        if (sucDiv)  sucDiv.style.display = 'none';
    }

    /* ── Button wiring ─────────────────────────────────────────── */
    var btnOpen = document.getElementById('btnOpenMovieDrawer');
    var btnOpenEmpty = document.getElementById('btnOpenMovieDrawerEmpty');

    var btnClose = document.getElementById('btnCloseMovieDrawer');
    if (btnClose) btnClose.addEventListener('click', closeDrawer);

    var btnBack = document.getElementById('btnMovieDrawerBack');
    if (btnBack)  btnBack.addEventListener('click', closeDrawer);

    if (overlay)  overlay.addEventListener('click', closeDrawer);

    document.addEventListener('keydown', function (e) {
        if (e.key === 'Escape') closeDrawer();
    });

    /* ── Image preview strip ───────────────────────────────────── */
    var imgInput   = document.getElementById('movieImageInput');
    var previewStrip = document.getElementById('moviePreviewStrip');

    if (imgInput && previewStrip) {
        imgInput.addEventListener('change', function () {
            previewStrip.innerHTML = '';
            var files = Array.from(this.files);
            files.forEach(function (file) {
                var reader = new FileReader();
                reader.onload = function (e) {
                    var img = document.createElement('img');
                    img.src = e.target.result;
                    img.style.cssText = 'width:64px;height:64px;object-fit:cover;border-radius:6px;border:1px solid #EAECF0;';
                    previewStrip.appendChild(img);
                };
                reader.readAsDataURL(file);
            });
        });
    }

    /* ── Genre chip selection ──────────────────────────────────── */
    document.addEventListener('click', function (e) {
        var chip = e.target.closest('#genreChipContainer .amenity-chip-item');
        if (chip) {
            chip.classList.toggle('selected');
            syncGenreHiddenInputs();
        }
    });

    function syncGenreHiddenInputs() {
        var container = document.getElementById('genreHiddenInputs');
        if (!container) return;
        container.innerHTML = '';
        var selected = document.querySelectorAll('#genreChipContainer .amenity-chip-item.selected');
        selected.forEach(function (chip) {
            var id = chip.dataset.id;
            var input = document.createElement('input');
            input.type = 'hidden';
            input.name = 'NewMovie.GenreId';
            input.value = id;
            container.appendChild(input);
        });
    }

    /* ── Edit Button wiring ────────────────────────────────────── */
    var editButtons = document.querySelectorAll('.btn-edit-movie');
    editButtons.forEach(function(btn) {
        btn.addEventListener('click', function(e) {
            e.stopPropagation();
            var id = this.dataset.id;
            
            fetch('?handler=MovieForEditAjax&movieId=' + id)
                .then(function(r) { return r.json(); })
                .then(function(res) {
                    if (res && res.success) {
                        var d = res.data;
                        document.getElementById('EditMovieId').value = id;
                        document.querySelector('[name="NewMovie.MovieName"]').value = d.movieName || '';
                        document.querySelector('[name="NewMovie.MovieCode"]').value = d.movieCode || '';
                        document.querySelector('[name="NewMovie.MovieDescription"]').value = d.movieDescription || '';
                        document.querySelector('[name="NewMovie.Duration"]').value = d.duration || '';
                        document.querySelector('[name="NewMovie.ReleaseDate"]').value = d.releaseDate ? d.releaseDate.split('T')[0] : '';
                        document.querySelector('[name="NewMovie.CertificateId"]').value = d.certificateId || '';
                        document.querySelector('[name="NewMovie.Status"]').value = d.status ? "true" : "false";
                        document.querySelector('[name="NewMovie.Tags"]').value = d.tags || '';
                        document.querySelector('[name="NewMovie.TrailerUrl"]').value = d.trailerUrl || '';
                        document.querySelector('[name="NewMovie.TeaserUrl"]').value = d.teaserUrl || '';
                        
                        var theaterIdEl = document.querySelector('[name="NewMovie.TheaterId"]');
                        if (theaterIdEl) {
                            theaterIdEl.value = d.theaterId || '';
                        }

                        var genreIds = d.genreId || [];
                        document.querySelectorAll('#genreChipContainer .amenity-chip-item').forEach(function(c) {
                            c.classList.remove('selected');
                        });
                        genreIds.forEach(function(gid) {
                            var chip = document.querySelector('#genreChipContainer .amenity-chip-item[data-id="' + gid + '"]');
                            if(chip) chip.classList.add('selected');
                        });
                        
                        form.setAttribute('action', '?handler=UpdateMovieAjax');
                        if (title) title.textContent = 'Edit movie details';
                        if (saveLbl) saveLbl.textContent = 'Save changes';
                        
                        openDrawer();
                    } else {
                        alert(res.message || 'Error fetching movie details.');
                    }
                })
                .catch(function() {
                    alert('Error fetching movie details.');
                });
        });
    });
    if (btnOpen) btnOpen.addEventListener('click', function() {
        form.reset();
        document.getElementById('EditMovieId').value = "0";
        document.querySelectorAll('#genreChipContainer .amenity-chip-item').forEach(function(c) { c.classList.remove('selected'); });
        if (previewStrip) previewStrip.innerHTML = '';
        form.setAttribute('action', '?handler=CreateMovieAjax');
        if (title) title.textContent = 'Add new movie';
        if (saveLbl) saveLbl.textContent = 'Add new movie';
        openDrawer();
    });

    if (btnOpenEmpty) btnOpenEmpty.addEventListener('click', function() {
        if(btnOpen) btnOpen.click();
    });

    /* ── Form submit via AJAX ──────────────────────────────────── */
    if (form) {
        form.addEventListener('submit', function (e) {
            e.preventDefault();

            syncGenreHiddenInputs();

            // Hide previous feedback
            if (errDiv) { errDiv.style.display = 'none'; errDiv.innerHTML = ''; }
            if (sucDiv)  sucDiv.style.display = 'none';

            if (saveBtn) { saveBtn.disabled = true; }
            var originalText = saveLbl ? saveLbl.textContent : 'Saving...';
            if (saveLbl) saveLbl.textContent = 'Saving...';

            var fd = new FormData(form);

            // Get antiforgery token from form
            var token = (form.querySelector('[name="__RequestVerificationToken"]') || {}).value;

            var handlerUrl = form.getAttribute('action') || '?handler=CreateMovieAjax';

            fetch(handlerUrl, {
                method: 'POST',
                headers: token ? { 'RequestVerificationToken': token } : {},
                body: fd
            })
            .then(function (r) { return r.json(); })
            .then(function (res) {
                if (saveBtn) saveBtn.disabled = false;
                if (saveLbl) saveLbl.textContent = originalText;

                if (res && res.success) {
                    if (sucDiv) sucDiv.style.display = 'flex';
                    setTimeout(function () {
                        closeDrawer();
                        window.location.reload();
                    }, 1200);
                } else {
                    var msgs = [];
                    if (res.errors) {
                        Object.values(res.errors).forEach(function (arr) {
                            arr.forEach(function (m) { if (m) msgs.push(m); });
                        });
                    }
                    if (!msgs.length) msgs.push('Unable to save movie.');
                    if (errDiv) {
                        errDiv.innerHTML = '<ul style="margin:0;padding-left:18px;">' +
                            msgs.map(function (m) { return '<li>' + m + '</li>'; }).join('') +
                            '</ul>';
                        errDiv.style.display = 'block';
                    }
                }
            })
            .catch(function () {
                if (saveBtn) saveBtn.disabled = false;
                if (saveLbl) saveLbl.textContent = originalText;
                if (errDiv) {
                    errDiv.innerHTML = 'A server error occurred. Please try again.';
                    errDiv.style.display = 'block';
                }
            });
        });
    }

    /* ── Client-side search ────────────────────────────────────── */
    if (srchInp) {
        srchInp.addEventListener('input', function () {
            var q = this.value.toLowerCase();
            var cards = document.querySelectorAll('#movieGrid .theater-card');
            cards.forEach(function (card) {
                var name = (card.dataset.name || '').toLowerCase();
                card.style.display = name.includes(q) ? '' : 'none';
            });
        });
    }

}());
