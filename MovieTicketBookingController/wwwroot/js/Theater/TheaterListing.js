/* ===========================================================
   THEATER LISTING — JavaScript
   jQuery $.ajax form submission + drawer control + live search
=========================================================== */

$(function () {
    'use strict';

    /* ─── Element refs ──────────────────────────────────────── */
    var $overlay = $('#drawerOverlay');
    var $drawer = $('#theaterDrawer');
    var $btnOpen = $('#btnOpenDrawer');
    var $btnOpenEmpty = $('#btnOpenDrawerEmpty');
    var $btnClose = $('#btnCloseDrawer');
    var $btnBack = $('#btnDrawerBack');
    var $saveBtn = $('#btnDrawerSave');
    var $saveBtnLabel = $('#btnDrawerSaveLabel');
    var $form = $('#drawerForm');
    var $errorSummary = $('#drawerErrorSummary');
    var $successBanner = $('#drawerSuccessBanner');
    var $statusSelect = $('#statusSelect');
    var $statusDot = $('#statusDot');
    var $imageInput = $('#drawerImageInput');
    var $previewStrip = $('#drawerPreviewStrip');
    var $searchInput = $('#theaterSearchInput');
    var $grid = $('#theaterGrid');

    /* ─────────────────────────────────────────────────────────
       DRAWER  —  Open / Close
    ───────────────────────────────────────────────────────── */

    function openDrawer() {
        $drawer.addClass('is-open');
        $overlay.addClass('is-open');
        $('body').css('overflow', 'hidden');

        // Focus first input after transition
        setTimeout(function () {
            $drawer.find('input, select, textarea').first().trigger('focus');
        }, 330);
    }

    function closeDrawer() {
        $drawer.removeClass('is-open');
        $overlay.removeClass('is-open');
        $('body').css('overflow', '');
    }

    $btnOpen.on('click', openDrawer);
    $btnOpenEmpty.on('click', openDrawer);
    $btnClose.on('click', closeDrawer);
    $btnBack.on('click', closeDrawer);
    $overlay.on('click', closeDrawer);

    $(document).on('keydown', function (e) {
        if (e.key === 'Escape') closeDrawer();
    });

    /* ─────────────────────────────────────────────────────────
       STATUS DOT
    ───────────────────────────────────────────────────────── */

    function syncStatusDot() {
        if ($statusSelect.val() === 'true') {
            $statusDot.removeClass('inactive');
        } else {
            $statusDot.addClass('inactive');
        }
    }

    $statusSelect.on('change', syncStatusDot);
    syncStatusDot();

    /* ─────────────────────────────────────────────────────────
       IMAGE UPLOAD  —  Preview Strip
    ───────────────────────────────────────────────────────── */

    $imageInput.on('change', function () {
        $previewStrip.empty();
        $.each(this.files, function (i, file) {
            var reader = new FileReader();
            reader.onload = function (e) {
                var $item = $('<div class="drawer-preview-item"></div>');
                var $img = $('<img>').attr({ src: e.target.result, alt: file.name });
                $item.append($img);
                $previewStrip.append($item);
            };
            reader.readAsDataURL(file);
        });
    });

    /* ─────────────────────────────────────────────────────────
       VALIDATION HELPERS
    ───────────────────────────────────────────────────────── */

    function clearErrors() {
        // Hide summary
        $errorSummary.hide().empty();

        // Clear field-level spans
        $form.find('[data-valmsg-for]').text('').hide();

        // Remove highlight from inputs/selects
        $form.find('.is-invalid').removeClass('is-invalid');
    }

    function showErrors(errors) {
        if (!errors) return;

        var summaryItems = [];

        $.each(errors, function (key, messages) {
            var msg = Array.isArray(messages) ? messages.join(' ') : messages;

            if (!msg) return;

            if (key === '' || key === null) {
                // General / summary error
                summaryItems.push(msg);
            } else {
                // Field-level: find the span by data-valmsg-for
                var $span = $form.find('[data-valmsg-for="' + key + '"]');

                if ($span.length) {
                    $span.text(msg).show();
                    // Highlight the adjacent input/select
                    var fieldId = key.replace(/\./g, '_');
                    $('#' + fieldId).addClass('is-invalid');
                } else {
                    // Unmapped field — push to summary
                    summaryItems.push(msg);
                }
            }
        });

        if (summaryItems.length > 0) {
            var html = summaryItems.map(function (m) {
                return '<p style="margin:0 0 4px">' + $('<div>').text(m).html() + '</p>';
            }).join('');
            $errorSummary.html(html).show();
        }
    }

    /* ─────────────────────────────────────────────────────────
       BUTTON  —  Loading State
    ───────────────────────────────────────────────────────── */

    function setLoading(on) {
        if (on) {
            $saveBtn.addClass('is-loading');
            $saveBtnLabel.html('<span class="btn-spinner"></span>Saving&hellip;');
        } else {
            $saveBtn.removeClass('is-loading');
            $saveBtnLabel.text('Add new theater');
        }
    }

    /* ─────────────────────────────────────────────────────────
       FORM  —  jQuery AJAX Submission
    ───────────────────────────────────────────────────────── */

    $form.on('submit', function (e) {
        e.preventDefault();

        // Clear any previous errors
        clearErrors();
        $successBanner.hide();

        // Build FormData (includes __RequestVerificationToken automatically)
        var formData = new FormData(this);

        // Enter loading state
        setLoading(true);

        $.ajax({
            url: '?handler=CreateTheaterAjax',
            type: 'POST',
            data: formData,
            processData: false,     // Do NOT let jQuery transform the data
            contentType: false,     // Let the browser set multipart boundary
            success: function (response) {
                if (response && response.success) {
                    // ── SUCCESS ─────────────────────────────────
                    setLoading(false);

                    // Show inline success banner
                    $successBanner.fadeIn(200);

                    // Scroll drawer body to top so banner is visible
                    $('.drawer__body').animate({ scrollTop: 0 }, 200);

                    // Close drawer after a moment, then reload the listing
                    setTimeout(function () {
                        closeDrawer();
                        location.reload();
                    }, 1200);

                } else {
                    // ── VALIDATION / SERVER ERRORS ───────────────
                    setLoading(false);
                    showErrors(response ? response.errors : null);

                    // Scroll drawer body to top so error summary is visible
                    $('.drawer__body').animate({ scrollTop: 0 }, 200);
                }
            },
            error: function (xhr) {
                setLoading(false);
                var msg = 'An unexpected error occurred. Please try again.';

                // Try to parse a meaningful message from the response
                try {
                    var body = JSON.parse(xhr.responseText);
                    if (body && body.title) msg = body.title;
                } catch (ex) { /* ignore */ }

                showErrors({ '': [msg] });
                $('.drawer__body').animate({ scrollTop: 0 }, 200);
            }
        });
    });

    /* ─────────────────────────────────────────────────────────
       LIVE SEARCH  —  Filter Theater Cards
    ───────────────────────────────────────────────────────── */

    $searchInput.on('input', function () {
        var term = $(this).val().toLowerCase().trim();
        var visibleCount = 0;

        $grid.find('.theater-card').each(function () {
            var name = ($(this).data('name') || '').toLowerCase();
            var text = $(this).text().toLowerCase();
            var matches = term === '' || name.includes(term) || text.includes(term);

            $(this).toggle(matches);
            if (matches) visibleCount++;
        });

        $('.record-count').text(visibleCount + ' Theater(s)');
    });

    /* ─────────────────────────────────────────────────────────
       EDIT MODE — Populate Drawer from Card Data Attributes
    ───────────────────────────────────────────────────────── */

    $(document).on('click', '.btn-edit-card', function () {
        var $btn = $(this);

        // Read data from the card's button
        var theaterId   = $btn.data('theater-id') || 0;
        var name        = $btn.data('theater-name') || '';
        var code        = $btn.data('theater-code') || '';
        var brand       = $btn.data('theater-brand') || '';
        var desc        = $btn.data('theater-description') || '';
        var status      = $btn.data('theater-status');
        var address1    = $btn.data('theater-address1') || '';
        var address2    = $btn.data('theater-address2') || '';
        var area        = $btn.data('theater-area') || '';
        var city        = $btn.data('theater-city') || '';
        var state       = $btn.data('theater-state') || '';
        var country     = $btn.data('theater-country') || '';
        var zip         = $btn.data('theater-zip') || '';
        var contactName = $btn.data('theater-contact-name') || '';
        var contactPhone= $btn.data('theater-contact-phone') || '';
        var ownerUserId = $btn.data('owner-user-id') || '';

        // Set hidden theater id
        $('#hdnTheaterId').val(theaterId);

        // Populate form fields
        $('#NewTheater_OwnerUserId').val(ownerUserId);
        $('#NewTheater_TheaterName').val(name);
        $('#NewTheater_TheaterCode').val(code);
        $('#NewTheater_TheaterBrand').val(brand);
        $('#NewTheater_TheaterDescription').val(desc);
        $('#NewTheater_AddressLine1').val(address1);
        $('#NewTheater_AddressLine2').val(address2);
        $('#NewTheater_Area').val(area);
        $('#NewTheater_City').val(city);
        $('#NewTheater_State').val(state);
        $('#NewTheater_Country').val(country);
        $('#NewTheater_ZipCode').val(zip);
        $('#NewTheater_ContactName').val(contactName);
        $('#NewTheater_ContactPhone').val(contactPhone);

        // Set status select
        $statusSelect.val(status === 'false' ? 'false' : 'true');
        syncStatusDot();

        // Switch form action to update handler
        $form.attr('action', '?handler=UpdateTheaterAjax');

        // Update drawer header
        $('#drawerTitle').text('Edit theater');
        $('#drawerSubtitle').text('Update the details for ' + name);

        // Update save button label
        $saveBtnLabel.text('Save changes');

        openDrawer();
    });

    // Reset to create mode when opening Add button
    $btnOpen.off('click').on('click', function () {
        $('#hdnTheaterId').val('0');
        $form.attr('action', '?handler=CreateTheaterAjax');
        $form[0].reset();
        $previewStrip.empty();
        $('#drawerTitle').text('Add new theater');
        $('#drawerSubtitle').text('Fill in the details to add a new theater');
        $saveBtnLabel.text('Add new theater');
        syncStatusDot();
        openDrawer();
    });

    $btnOpenEmpty.off('click').on('click', function () {
        $('#hdnTheaterId').val('0');
        $form.attr('action', '?handler=CreateTheaterAjax');
        $form[0].reset();
        $previewStrip.empty();
        $('#drawerTitle').text('Add new theater');
        $('#drawerSubtitle').text('Fill in the details to add a new theater');
        $saveBtnLabel.text('Add new theater');
        syncStatusDot();
        openDrawer();
    });

});