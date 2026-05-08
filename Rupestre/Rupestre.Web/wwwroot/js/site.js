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
