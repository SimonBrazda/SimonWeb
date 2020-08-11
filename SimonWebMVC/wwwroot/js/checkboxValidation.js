$.validator.addMethod("mustbetrue",
    function (value, element, parameters) {
        return element.checked;
    });

$.validator.unobtrusive.adapters.add("mustbetrue", [], function (options) {
    options.rules.mustbetrue = {};
    options.messages["mustbetrue"] = options.message;
});