window.TicketModel = Backbone.Model.extend({

});

window.TicketListCollection = Backbone.Collection.extend({
    model: window.TicketModel,
    url: window.urls.ticketApiUrl
});

window.TicketListView = Backbone.View.extend({
    initialize: function (attributes) {
        _.bindAll(this);
        this.template = _.template($("#listTemplate").html());
        this.collection.bind("reset", this.render);
        this.collection.bind("change:name", this.render);
        this.collection.bind("add", this.render);
        this.collection.bind("remove", this.render);
        this.router = attributes.router;
        
        window.Bus.on("ticket:created", _.bind(function(ticket) {
            this.collection.add(ticket);
        }, this));
    },

    events: {
        "click #createTicketLink": "_createNew"
    },
    
    render: function () {
        var data = { items: this.collection.toJSON() };
        var html = this.template(data);
        $(this.el).html(html);        

        return this;
    },

    _createNew: function () {
        this.router.navigate("create", true);
        return false;
    }
});