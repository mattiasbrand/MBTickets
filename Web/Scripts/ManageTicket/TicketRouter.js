window.TicketRouter = Backbone.Router.extend({
    initialize: function () {
        var ticketListCollection = new window.TicketListCollection();
        this.ticketListView = new window.TicketListView({ collection: ticketListCollection, el: "#listView", router: this });
        this.ticketListView.collection.fetch();
        
        this.createTicketView = new window.CreateTicketView({ el: "#createView", router: this });

        window.Bus.on("ticket:created", _.bind(function () {
            this.navigate("#");
        }, this));
    },

    routes: {
        "": "index",
        "create": "createTicket"
    },

    index: function () {
        this.createTicketView.hide(300);
        this.ticketListView.delay(300).show(300);
    },

    createTicket: function () {
        this.createTicketView.reset();
        this.ticketListView.hide(300);
        this.createTicketView.delay(300).show(300);
    }
});