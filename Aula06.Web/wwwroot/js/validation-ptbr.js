// Ajusta o jQuery Validation para aceitar vírgula como separador decimal (pt-BR),
// já que o servidor (cultura pt-BR) espera "349,90" e não "349.90".
(function ($) {
    if (!$ || !$.validator) {
        return;
    }

    function parsePtBrNumber(value) {
        return parseFloat(value.replace(/\./g, "").replace(",", "."));
    }

    $.validator.methods.number = function (value, element) {
        return this.optional(element) || /^-?\d{1,3}(\.\d{3})*(,\d+)?$|^-?\d+(,\d+)?$/.test(value);
    };

    $.validator.methods.range = function (value, element, param) {
        var num = parsePtBrNumber(value);
        return this.optional(element) || (num >= param[0] && num <= param[1]);
    };
})(window.jQuery);
