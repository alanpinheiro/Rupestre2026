window.showAlert = function (message, type) {
    type = type || 'danger';
    let container = document.getElementById('_toastContainer');
    if (!container) {
        container = document.createElement('div');
        container.id = '_toastContainer';
        container.className = 'toast-container position-fixed top-0 end-0 p-3';
        container.style.zIndex = '9999';
        document.body.appendChild(container);
    }
    const icons = { danger: 'bi-x-circle-fill', warning: 'bi-exclamation-triangle-fill', success: 'bi-check-circle-fill', info: 'bi-info-circle-fill' };
    const el = document.createElement('div');
    el.className = 'toast align-items-center text-bg-' + type + ' border-0';
    el.setAttribute('role', 'alert');
    el.innerHTML = '<div class="d-flex"><div class="toast-body"><i class="bi ' + (icons[type] || icons.danger) + ' me-2"></i>' + message + '</div>'
        + '<button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button></div>';
    container.appendChild(el);
    const toast = new bootstrap.Toast(el, { delay: 5000 });
    toast.show();
    el.addEventListener('hidden.bs.toast', function () { el.remove(); });
};

window.showConfirm = function (message, onConfirm) {
    let el = document.getElementById('_confirmModal');
    if (!el) {
        el = document.createElement('div');
        el.id = '_confirmModal';
        el.className = 'modal fade';
        el.tabIndex = -1;
        el.innerHTML = '<div class="modal-dialog modal-dialog-centered modal-sm">'
            + '<div class="modal-content">'
            + '<div class="modal-body text-center pt-4 pb-2">'
            + '<i class="bi bi-exclamation-triangle-fill text-warning d-block mb-3" style="font-size:2.5rem"></i>'
            + '<p id="_confirmMsg" class="mb-0"></p></div>'
            + '<div class="modal-footer justify-content-center border-0 pb-4">'
            + '<button type="button" class="btn btn-secondary btn-sm" data-bs-dismiss="modal">Cancelar</button>'
            + '<button type="button" class="btn btn-danger btn-sm" id="_confirmOk">Confirmar</button>'
            + '</div></div></div>';
        document.body.appendChild(el);
    }
    document.getElementById('_confirmMsg').textContent = message;
    const bsModal = new bootstrap.Modal(el);
    const old = document.getElementById('_confirmOk');
    const btn = old.cloneNode(true);
    old.parentNode.replaceChild(btn, old);
    btn.addEventListener('click', function () { bsModal.hide(); onConfirm(); });
    bsModal.show();
};

// Inicializa Select2 em todos os <select> dentro do escopo informado,
// ignorando controles do DataTables e selects já inicializados.
window.initSelect2 = function (scope) {
    const $scope = $(scope instanceof Element ? scope : document);

    $scope.find('select').each(function () {
        const $el = $(this);
        if ($el.data('select2')) return;                        // já inicializado
        if ($el.closest('.dataTables_wrapper').length) return;  // controle DataTables
        if ($el.data('no-select2') !== undefined) return;       // opt-out explícito

        const $modal = $el.closest('.modal');

        $el.select2({
            theme: 'bootstrap-5',
            width: '100%',
            language: { noResults: () => 'Nenhum resultado encontrado' },
            allowClear: $el.find('option[value=""]').length > 0,
            placeholder: $el.find('option[value=""]').first().text() || 'Selecione...',
            dropdownParent: $modal.length ? $modal : $(document.body)
        });
    });
};

// Inicializa em toda a página após o DOM estar pronto.
// O @section Scripts já executou de forma síncrona antes deste evento,
// garantindo que selects dinâmicos (Venda/Create) já estão populados.
document.addEventListener('DOMContentLoaded', function () {
    initSelect2(document);
});

// Reinicializa automaticamente quando um modal Bootstrap é exibido,
// cobrindo todos os formulários carregados via AJAX.
document.addEventListener('shown.bs.modal', function (e) {
    initSelect2(e.target);
});
