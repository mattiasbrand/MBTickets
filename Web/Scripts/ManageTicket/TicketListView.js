window.TicketModel = Backbone.Model.extend({

});

window.TicketListCollection = Backbone.Collection.extend({
    model: window.TicketModel,
    url: window.urls.ticketApiUrl
});

window.TicketListView = Backbone.View.extend({
    initialize: function (attributes) {
        _.bindAll(this);
        this.collection.bind("reset", this.reset);
        this.router = attributes.router;
        this.content = this.$("#listContent");

        window.Bus.on("ticket:created", _.bind(function (ticket) {
            this.add(ticket);
        }, this));
    },

    events: {
        "click #createTicketLink": "_createNew"
    },

    reset: function () {
        this.content.empty();
        this.render();
    },

    render: function () {
        _(this.collection.models).each(this.add, this);
        return this;
    },

    add: function (ticket) {
        var itemView = new window.TicketListItemView({ model: ticket });
        this.content.append(itemView.render().el);
    },

    _createNew: function () {
        this.router.navigate("create", true);
        return false;
    }
});