jQuery(function ($) {
	$.validator.addMethod('number', function (value, element) {
		return this.optional(element) || /^(?:-?\d+)(?:(\.|,)\d+)?$/.test(value);
	})

	$.validator.addMethod('range', function (value, element, param) {
		var globalizedValue = value.replace(",", ".");
		return this.optional(element) || (globalizedValue >= param[0] && globalizedValue <= param[1]);
	})
});