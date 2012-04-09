window.TicketListItemView = Backbone.View.extend({
    tagName: 'tr',

    initialize: function () {
        _.bindAll(this);
        this.template = _.template($("#listItemTemplate").html());
        this.model.bind('change', this.render, this);
        this.model.bind('destroy', this.remove, this);
    },

    events: {
        'click .deleteTicket': 'destroy'
    },

    render: function () {
        $(this.el).html(this.template(this.model.toJSON()));

        return this;
    },

    destroy: function (e) {
        if (confirm('Are you sure you want to delete this?')) {            
            $.ajax({
                url: window.urls.ticketApiUrl,
                type: "DELETE",
                data: { id: this.model.get("Id") },
                success: function (response) {
                    //window.Bus.trigger("ticket:deleted", this.model);
                    this.model.destroy();
                    $.gritter.add({ title: "Deleted", text: "Ticket deleted." });
                } .bind(this),
                error: function (xhr, status, error) {
                    $.gritter.add({ title: "Error!", text: "Error on server when deleting ticket.", sticky: true });
                }
            });
        }
        return false;
    },

    remove: function () {
        $(this.el).remove();
    }
});