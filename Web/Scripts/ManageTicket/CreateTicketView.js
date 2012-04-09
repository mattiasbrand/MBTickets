window.CreateTicketView = Backbone.View.extend({
    initialize: function (attributes) {
        _.bindAll(this);
        this.model = window.TicketModel.create;
        this.router = attributes.router;
        this.title = $("#newTitle");
        this.description = $("#newDescription");
        this.saveButton = $("#saveNew");
    },

    events: {
        "click #saveNew": "_save",
        "click #listTicketsLink": "_listTickets"
    },

    reset: function () {
        this.model = new TicketModel();
        this.render();
    },

    render: function () {
        this.title.val(this.model.get("title"));
        this.description.val(this.model.get("description"));
        return this;
    },

    _save: function () {
        var data = { Id: window.common.newGuid(), Creator: $("a.username").text(), Title: this.title.val(), Description: this.description.val() };
        this.model.set(data); // add selected data to model

        $.ajax({
            url: window.urls.ticketApiUrl,
            type: "PUT",
            data: this.model.toJSON(),
            success: function (response) {
                window.Bus.trigger("ticket:created", this.model);
                $.gritter.add({ title: "Created", text: "Ticket successfully created." });
            } .bind(this),
            error: function (xhr, status, error) {
                $.gritter.add({ title: "Error!", text: "Error on server when creating ticket.", sticky: true });
            }
        });

        window.history.back();
    },

    _listTickets: function () {
        this.router.navigate("#", true);
        return false;
    }
});