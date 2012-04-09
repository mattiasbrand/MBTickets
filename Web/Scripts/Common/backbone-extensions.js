window.Backbone.View.prototype.show = function(args) {
    return $(this.el).show(args);
};

window.Backbone.View.prototype.delay = function (args) {
    return $(this.el).delay(args);
};

window.Backbone.View.prototype.hide = function (args) {
    return $(this.el).hide(args);
};