toastr.options = { closeButton: true, progressBar: true, positionClass: "toast-top-right", timeOut: 5000 };

window.NOTIFY = {
    toast: (type, message, title) => toastr[type](message, title), // type: success | info | warning | error
    alert: (icon, title, text) => Swal.fire({ icon, title, text }),
    confirm: async (title, text) =>
    {
        const r = await Swal.fire({
            title, text, icon: "warning",
            showCancelButton: true, confirmButtonText: "Delete", confirmButtonColor: "#d33"
        });
        return r.isConfirmed;
    },
    addPasswords: async () =>
    {
        const rowHtml = () => `
        <tr>
            <td><input class="pw-input pw-title" placeholder="Title" autocomplete="off"></td>
            <td><input class="pw-input pw-user" placeholder="Username" autocomplete="off"></td>
            <td><input class="pw-input pw-pass" placeholder="Password" autocomplete="off"></td>
            <td><button type="button" class="pw-remove" title="Remove row">✕</button></td>
        </tr>`;

        const r = await Swal.fire({
            title: "Add passwords",
            width: "60rem",
            html: `
            <style>
                .pw-table { width: 100%; border-collapse: collapse; }
                .pw-table th { text-align: left; padding: 4px; font-size: .9rem; }
                .pw-table td { padding: 4px; }
                .pw-input { width: 100%; padding: 6px 8px; border: 1px solid #ccc; border-radius: 4px; font-size: 1rem; }
                .add-rows-input { width: 100%; padding: 6px 8px; border: 1px solid #ccc; border-radius: 4px; font-size: 1rem; }
                .pw-remove { border: none; background: #f44336; color: #fff; border-radius: 4px; padding: 6px 10px; cursor: pointer; }
                .pw-add { margin-top: 8px; border: none; background: #4caf50; color: #fff; border-radius: 4px; padding: 6px 12px; cursor: pointer; }
            </style>
            <div class="row">
                <div class="col-8 p-2">
                    <input class="add-rows-input" id="num-rows" type="number" max="40" min="0" step="1" placeholder="Enter a number between 1 and 39">
                </div>
                <div class="col-4 p-2">
                    <button id="add-rows" class="btn btn-info">Add rows</button>
                </div>
            </div>
            <table class="pw-table">
                <thead><tr><th>Title *</th><th>Username</th><th>Password *</th><th></th></tr></thead>
                <tbody id="pw-rows">${rowHtml()}</tbody>
            </table>
            <button type="button" class="pw-add" id="pw-add">+ Add row</button>`,
            showCancelButton: true,
            confirmButtonText: "Save all",
            focusConfirm: false,
            didOpen: (popup) =>
            {
                const tbody = $(popup).find("#pw-rows")
                const addRowBtn = $('#pw-add')
                const removeRowBtn = $('.pw-remove')
                const addrowsInput = $('#num-rows')
                const bindEventHandlers = () => {
                    const latestAddedRowRemoveBtn = tbody.children().last().find(".pw-remove")
                    latestAddedRowRemoveBtn.trigger('focus')
                    latestAddedRowRemoveBtn.on("click", function () 
                    {
                        if (tbody.children('tr').length > 1)
                        {
                            $(this).closest('tr').remove()
                        }
                    });
                }
                $('#add-rows').on('click',function(){
                    // get number of rows user entered and add to table
                    const numRows = addrowsInput.val()
                    if (numRows > 39 || numRows < 1)
                    {
                        toastr.error('Please enter a number between 1 and 39')
                        return;
                    }
                    if (tbody.children('tr').length > 40)
                    {
                        toastr.error('You can only add up to 40 rows at a time')
                        return;
                    }
                    for (let i=0;i<numRows;i++)
                    {
                        tbody.append(rowHtml())
                        bindEventHandlers()
                    }
                    addrowsInput.val('')
                })
                addRowBtn.on("click", function ()
                {
                    tbody.append(rowHtml())
                    bindEventHandlers()
                });
                removeRowBtn.on("click", function () 
                {
                    if (tbody.children('tr').length > 1)
                    {
                        $(this).closest('tr').remove()
                    }
                });
                tbody.find(".pw-title").trigger('focus')
            },
            preConfirm: () =>
            {
                const rows = [...Swal.getPopup().querySelectorAll("#pw-rows tr")]
                    .map(tr => ({
                        title: tr.querySelector(".pw-title").value.trim(),
                        username: tr.querySelector(".pw-user").value.trim(),
                        password: tr.querySelector(".pw-pass").value,
                    }))
                    .filter(x => x.title || x.username || x.password); // skip blank rows

                if (rows.length === 0)
                {
                    Swal.showValidationMessage("Add at least one password");
                    return false;
                }
                const bad = rows.findIndex(x => !x.title || !x.password);
                if (bad !== -1)
                {
                    Swal.showValidationMessage(`Row ${bad + 1}: Title and Password are required`);
                    return false;
                }
                return rows;
            }
        });

        return r.isConfirmed ? r.value : null;
    },
};
