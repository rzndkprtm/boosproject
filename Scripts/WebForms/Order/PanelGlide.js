let designIdOri = "6";
let itemAction;
let headerId;
let itemId;
let designId;
let company;
let companyDetail;
let loginId;
let roleAccess;
let priceAccess;

initPanelGlide();

$("#submit").on("click", process);
$("#cancel").on("click", () => window.location.href = `/order/detail?orderid=${headerId}`);
$("#vieworder").on("click", () => window.location.href = `/order/detail?orderid=${headerId}`);

$("#blindtype").on("change", function () {
    bindTubeType($(this).val());
    bindMounting($(this).val());
    visibleProduct($(this).val());
    bindLayoutCode(blindtype, $("#tracktype").val());

    document.getElementById("wandlength").value = "";
});

$("#tubetype").on("change", function () {
    const blindtype = document.getElementById("blindtype").value;
    bindColourType(blindtype, $(this).val());
    bindFabricType(designId, $(this).val());

    document.getElementById("wandlength").value = "";
});

$("#colourtype").on("change", function () {
    const blindtype = document.getElementById("blindtype").value;
    const tubetype = document.getElementById("tubetype").value;
    visibleDetail(blindtype, tubetype, $(this).val());

    const wandlength = document.getElementById("wandlength").value;

    visibleWandLength(wandlength);
});

$("#fabrictype").on("change", function () {
    bindFabricColour($(this).val());
});

$("#tracktype").on("change", function () {
    const layoutCode = document.getElementById("layoutcode").value;

    bindLayoutCode($("#blindtype").val(), $(this).val());

    bindPanelQty($(this).val(), layoutCode);
});

$("#layoutcode").on("change", function () {
    const tracktype = document.getElementById("tracktype").value;

    visibleLayoutCustom($(this).val());

    bindPanelQty(tracktype, $(this).val());
});

$("#wandlength").on("change", function () {
    visibleWandLength($(this).val());
});

function loader(itemAction) {
    return new Promise((resolve) => {
        if (itemAction === "create") {
            document.getElementById("divloader").style.display = "none";
            document.getElementById("divorder").style.display = "";
        }
        resolve();
    });
}

function isError(msg) {
    $("#modalError").modal("show");
    document.getElementById("errorMsg").innerHTML = msg;
}

function getOrderHeader(headerId) {
    return new Promise((resolve, reject) => {
        if (!headerId) return resolve();

        $.ajax({
            type: "POST",
            url: "Method.aspx/GetOrderHeader",
            data: JSON.stringify({ headerId }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: ({ d }) => {
                document.getElementById("orderid").innerText = d.OrderId || "-";
                document.getElementById("ordernumber").innerText = d.OrderNumber || "-";
                document.getElementById("ordername").innerText = d.OrderName || "-";
                resolve(d);
            },
            error: reject
        });
    });
}

function getFormAction(itemAction) {
    return new Promise((resolve) => {
        const pageAction = document.getElementById("pageaction");
        if (!pageAction) {
            resolve();
            return;
        }

        const actionMap = {
            create: "Add Item",
            edit: "Edit Item",
            view: "View Item",
            copy: "Copy Item"
        };
        pageAction.innerText = actionMap[itemAction];
        resolve();
    });
}

function getBlindName(blindType) {
    if (!blindType) return;

    const type = "BlindName";
    return new Promise((resolve, reject) => {
        $.ajax({
            type: "POST",
            url: "Method.aspx/StringData",
            data: JSON.stringify({ type: type, dataId: blindType }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                resolve(response.d);
            },
            error: function (error) {
                reject(error);
            }
        });
    });
}

function getTubeName(tubeType) {
    if (!tubeType) return;

    const type = "TubeName";
    return new Promise((resolve, reject) => {
        $.ajax({
            type: "POST",
            url: "Method.aspx/StringData",
            data: JSON.stringify({ type: type, dataId: tubeType }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                resolve(response.d);
            },
            error: function (error) {
                reject(error);
            }
        });
    });
}

function getCompanyOrder(headerId) {
    return new Promise((resolve, reject) => {
        company = "";

        if (!headerId) {
            resolve();
            return;
        }

        const type = "CompanyOrder";
        $.ajax({
            type: "POST",
            url: "Method.aspx/StringData",
            data: JSON.stringify({ type: type, dataId: headerId }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                company = response.d.trim();
                resolve();
            },
            error: function (error) {
                reject(error);
            }
        });
    });
}

function getCompanyDetailOrder(headerId) {
    return new Promise((resolve, reject) => {
        companyDetail = "";

        if (!headerId) {
            resolve();
            return;
        }

        const type = "CompanyDetailOrder";
        $.ajax({
            type: "POST",
            url: "Method.aspx/StringData",
            data: JSON.stringify({ type: type, dataId: headerId }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                companyDetail = response.d.trim();
                resolve();
            },
            error: function (error) {
                reject(error);
            }
        });
    });
}

function getRoleAccess(loginId) {
    return new Promise((resolve, reject) => {
        roleAccess = "";

        if (!loginId) {
            resolve();
            return;
        }

        const type = "RoleAccess";
        $.ajax({
            type: "POST",
            url: "Method.aspx/StringData",
            data: JSON.stringify({ type: type, dataId: loginId }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                roleAccess = response.d.trim();
                resolve();
            },
            error: function (error) {
                reject(error);
            }
        });
    });
}

function getPriceAccess(loginId) {
    return new Promise((resolve, reject) => {
        priceAccess = "";

        if (!loginId) {
            resolve();
            return;
        }

        const type = "CustomerPriceAccess";
        $.ajax({
            type: "POST",
            url: "Method.aspx/StringData",
            data: JSON.stringify({ type: type, dataId: loginId }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                priceAccess = response.d.trim();
                resolve();
            },
            error: function (error) {
                reject(error);
            }
        });
    });
}

function getDesignName(designType) {
    return new Promise((resolve, reject) => {
        const cardTitle = document.getElementById("cardtitle");
        cardTitle.textContent = "";

        if (!designType) {
            resolve();
            return;
        }

        const type = "DesignName";
        $.ajax({
            type: "POST",
            url: "Method.aspx/StringData",
            data: JSON.stringify({ type: type, dataId: designType }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                const designName = response.d.trim();
                cardTitle.textContent = designName;
                resolve();
            },
            error: function (error) {
                reject(error);
            }
        });
    });
}

function bindBlindType(designType) {
    return new Promise((resolve, reject) => {
        const blindtype = document.getElementById("blindtype");
        blindtype.innerHTML = "";

        if (!designType) {
            const selectedValue = blindtype.value || "";
            Promise.all([
                visibleProduct(selectedValue),
                bindTubeType(selectedValue),
                bindMounting(selectedValue)
            ]).then(resolve).catch(reject);
            return;
        }

        const listData = {
            type: "BlindType",
            companydetail: companyDetail,
            designtype: designType
        };

        $.ajax({
            type: "POST",
            url: "Method.aspx/ListData",
            data: JSON.stringify({ data: listData }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (Array.isArray(response.d)) {
                    blindtype.innerHTML = "";

                    if (response.d.length > 1) {
                        const defaultOption = document.createElement("option");
                        defaultOption.text = "";
                        defaultOption.value = "";
                        blindtype.add(defaultOption);
                    }

                    response.d.forEach(function (item) {
                        const option = document.createElement("option");
                        option.value = item.Value;
                        option.text = item.Text;
                        blindtype.add(option);
                    });

                    if (response.d.length === 1) {
                        blindtype.selectedIndex = 0;
                    }

                    const selectedValue = blindtype.value || "";
                    Promise.all([
                        visibleProduct(selectedValue),
                        bindTubeType(selectedValue),
                        bindMounting(selectedValue)
                    ]).then(resolve).catch(reject);
                } else {
                    const selectedValue = blindtype.value || "";
                    Promise.all([
                        visibleProduct(selectedValue),
                        bindTubeType(selectedValue),
                        bindMounting(selectedValue)
                    ]).then(resolve).catch(reject);
                }
            },
            error: function (error) {
                reject(error);
            }
        });
    });
}

function bindTubeType(blindType) {
    return new Promise((resolve, reject) => {
        const tubetype = document.getElementById("tubetype");
        tubetype.innerHTML = "";

        if (!blindType) {
            const selectedValue = tubetype.value || "";
            Promise.all([
                bindFabricType(designId, selectedValue),
                bindColourType(blindType, selectedValue)
            ]).then(resolve).catch(reject);
            return;
        }

        let listData = { type: "TubeType", companydetail: companyDetail, blindtype: blindType };

        $.ajax({
            type: "POST",
            url: "Method.aspx/ListData",
            data: JSON.stringify({ data: listData }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (Array.isArray(response.d)) {
                    tubetype.innerHTML = "";

                    if (response.d.length > 1) {
                        const defaultOption = document.createElement("option");
                        defaultOption.text = "";
                        defaultOption.value = "";
                        tubetype.add(defaultOption);
                    }

                    response.d.forEach(function (item) {
                        const option = document.createElement("option");
                        option.value = item.Value;
                        option.text = item.Text;
                        tubetype.add(option);
                    });

                    if (response.d.length === 1) {
                        tubetype.selectedIndex = 0;
                    }

                    const selectedValue = tubetype.value || "";
                    Promise.all([
                        bindFabricType(designId, selectedValue),
                        bindColourType(blindType, selectedValue)
                    ]).then(resolve).catch(reject);
                } else {
                    const selectedValue = tubetype.value || "";
                    Promise.all([
                        bindFabricType(designId, selectedValue),
                        bindColourType(blindType, selectedValue)
                    ]).then(resolve).catch(reject);
                }
            },
            error: function (error) {
                reject(error);
            }
        });
    });
}

function bindColourType(blindType, tubeType) {
    return new Promise((resolve, reject) => {
        const colourtype = document.getElementById("colourtype");
        colourtype.innerHTML = "";

        if (!blindType || !tubeType) {
            const selectedValue = colourtype.value || "";
            Promise.resolve(
                visibleDetail(blindType, tubeType, selectedValue)
            ).then(resolve).catch(reject);
            return;
        }

        const listData = { type: "ColourType", companydetail: companyDetail, blindtype: blindType, tubetype: tubeType, controltype: "0" };

        $.ajax({
            type: "POST",
            url: "Method.aspx/ListData",
            data: JSON.stringify({ data: listData }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (Array.isArray(response.d)) {
                    colourtype.innerHTML = "";

                    if (response.d.length > 1) {
                        const defaultOption = document.createElement("option");
                        defaultOption.text = "";
                        defaultOption.value = "";
                        colourtype.add(defaultOption);
                    }

                    response.d.forEach(function (item) {
                        const option = document.createElement("option");
                        option.value = item.Value;
                        option.text = item.Text;
                        colourtype.add(option);
                    });

                    if (response.d.length === 1) {
                        colourtype.selectedIndex = 0;
                    }

                    const selectedValue = colourtype.value || "";
                    Promise.resolve(
                        visibleDetail(blindType, tubeType, selectedValue)
                    ).then(resolve).catch(reject);
                } else {
                    const selectedValue = colourtype.value || "";
                    Promise.resolve(
                        visibleDetail(blindType, tubeType, selectedValue)
                    ).then(resolve).catch(reject);
                }
            },
            error: function (error) {
                reject(error);
            }
        });
    });
}

function bindMounting(blindType) {
    return new Promise((resolve, reject) => {
        const mounting = document.getElementById("mounting");

        if (!blindType) {
            resolve();
            return;
        }

        const listData = { type: "Mounting", blindtype: blindType };

        $.ajax({
            type: "POST",
            url: "Method.aspx/ListData",
            data: JSON.stringify({ data: listData }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (Array.isArray(response.d)) {
                    mounting.innerHTML = "";

                    if (response.d.length > 1) {
                        const defaultOption = document.createElement("option");
                        defaultOption.text = "";
                        defaultOption.value = "";
                        mounting.add(defaultOption);
                    }

                    response.d.forEach(function (item) {
                        const option = document.createElement("option");
                        option.value = item.Value;
                        option.text = item.Text;
                        mounting.add(option);
                    });

                    if (response.d.length === 1) {
                        mounting.selectedIndex = 0;
                    }
                }
                resolve();
            },
            error: function (error) {
                reject(error);
            }
        });
    });
}

function bindFabricType(designType, tubeType) {
    return new Promise((resolve, reject) => {
        const fabrictype = document.getElementById("fabrictype");
        fabrictype.innerHTML = "";

        if (!designType || !tubeType) {
            const selectedValue = fabrictype.value || "";
            Promise.resolve(
                bindFabricColour(selectedValue)
            ).then(resolve).catch(reject);
            return;
        }

        const listData = { type: "FabricType", designtype: designType, companydetail: companyDetail, tubetype: tubeType };

        $.ajax({
            type: "POST",
            url: "Method.aspx/ListData",
            data: JSON.stringify({ data: listData }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (Array.isArray(response.d)) {
                    fabrictype.innerHTML = "";

                    if (response.d.length > 1) {
                        const defaultOption = document.createElement("option");
                        defaultOption.text = "";
                        defaultOption.value = "";
                        fabrictype.add(defaultOption);
                    }

                    response.d.forEach(function (item) {
                        const option = document.createElement("option");
                        option.value = item.Value;
                        option.text = item.Text;
                        fabrictype.add(option);
                    });

                    if (response.d.length === 1) {
                        fabrictype.selectedIndex = 0;
                    }

                    const selectedValue = fabrictype.value || "";
                    Promise.resolve(
                        bindFabricColour(selectedValue)
                    ).then(resolve).catch(reject);
                } else {
                    const selectedValue = fabrictype.value || "";
                    Promise.resolve(
                        bindFabricColour(selectedValue)
                    ).then(resolve).catch(reject);
                }
            },
            error: function (error) {
                reject(error);
            }
        });
    });
}

function bindFabricColour(fabricType) {
    return new Promise((resolve, reject) => {
        const fabriccolour = document.getElementById("fabriccolour");
        fabriccolour.innerHTML = "";

        if (!fabricType) {
            resolve();
            return;
        }

        const listData = { type: "FabricColour", fabrictype: fabricType };

        $.ajax({
            type: "POST",
            url: "Method.aspx/ListData",
            data: JSON.stringify({ data: listData }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (Array.isArray(response.d)) {
                    fabriccolour.innerHTML = "";

                    if (response.d.length > 1) {
                        const defaultOption = document.createElement("option");
                        defaultOption.text = "";
                        defaultOption.value = "";
                        fabriccolour.add(defaultOption);
                    }

                    response.d.forEach(function (item) {
                        const option = document.createElement("option");
                        option.value = item.Value;
                        option.text = item.Text;
                        fabriccolour.add(option);
                    });

                    if (response.d.length === 1) {
                        fabriccolour.selectedIndex = 0;
                    }
                }
                resolve();
            },
            error: function (error) {
                reject(error);
            }
        });
    });
}

function bindLayoutCode(blindType, trackType) {
    return new Promise((resolve) => {
        const layoutcode = document.getElementById("layoutcode");
        if (!layoutcode) {
            return resolve();
        }
        layoutcode.innerHTML = "";

        if (!blindType || !trackType) return resolve();

        let options = [{ value: "", text: "" }];

        getBlindName(blindType).then(blindName => {
            if (blindName === "Complete Set" || blindName === "Track Only") {
                options = [
                    { value: "", text: "" }, { value: "A", text: "A" },
                    { value: "B", text: "B" }, { value: "C", text: "C" },
                    { value: "D", text: "D" }, { value: "E", text: "E" },
                    { value: "F", text: "F" }, { value: "S", text: "S" }
                ];

                if (trackType === "2") {
                    options = [
                        { value: "", text: "" }, { value: "A", text: "A" },
                        { value: "B", text: "B" }, { value: "C", text: "C" },
                        { value: "D", text: "D" }, { value: "S", text: "S" }
                    ];
                }
            }

            const fragment = document.createDocumentFragment();
            options.forEach(opt => {
                const optionElement = document.createElement("option");
                optionElement.value = opt.value;
                optionElement.textContent = opt.text;
                fragment.appendChild(optionElement);
            });
            layoutcode.appendChild(fragment);
            resolve();
        }).catch(error => {
            resolve();
        });
    });
}

function bindPanelQty(trackType, layoutCode) {
    return new Promise((resolve) => {
        if (!trackType || !layoutCode) {
            document.getElementById("panelqty").value = '';
            return resolve();
        }

        let panelQty = trackType;
        if (layoutCode === "E" || layoutCode === "F") {
            panelQty = parseInt(trackType) + (parseInt(trackType) - 1);
        }

        document.getElementById("panelqty").value = panelQty;
        resolve();
    });
}

function visibleProduct(blindType) {
    return new Promise((resolve) => {
        const panelstyle = document.getElementById("divpanelstyle");
        const trackcolour = document.getElementById("divtrackcolour");

        function toggleDisplay(element, show) {
            if (element) element.style.display = show ? "" : "none";
        }

        toggleDisplay(panelstyle, false);
        toggleDisplay(trackcolour, false);

        if (!blindType) return resolve();

        getBlindName(blindType).then(blindName => {
            if (blindName === "Complete Set" || blindName === "Panel Only") {
                toggleDisplay(panelstyle, true);
            }

            if (blindName === "Complete Set" || blindName === "Track Only") {
                toggleDisplay(trackcolour, true);
            }

            resolve();
        }).catch(error => {
            resolve();
        });
    });
}

function visibleDetail(blindType, tubeType, colourType) {
    return new Promise((resolve) => {
        const detail = document.getElementById("divdetail");
        const mounting = document.getElementById("divmounting");
        const fabrictype = document.getElementById("divfabrictype");
        const fabriccolour = document.getElementById("divfabriccolour");
        const drop = document.getElementById("divdrop");
        const tracktype = document.getElementById("divtracktype");
        const wandlength = document.getElementById("divwandlength");
        const wandlengthvalue = document.getElementById("divwandlengthvalue");
        const layoutcode = document.getElementById("divlayoutcode");
        const layoutcustom = document.getElementById("divlayoutcustom");
        const panelqty = document.getElementById("divpanelqty");
        const battenfront = document.getElementById("divbattenfront");
        const markup = document.getElementById("divmarkup");

        function toggleDisplay(element, show) {
            if (element) element.style.display = show ? "" : "none";
        }

        toggleDisplay(detail, false);
        toggleDisplay(mounting, false);
        toggleDisplay(fabrictype, false);
        toggleDisplay(fabriccolour, false);
        toggleDisplay(drop, false);
        toggleDisplay(tracktype, false);
        toggleDisplay(wandlength, false);
        toggleDisplay(wandlengthvalue, false);
        toggleDisplay(layoutcode, false);
        toggleDisplay(layoutcustom, false);
        toggleDisplay(panelqty, false);
        toggleDisplay(battenfront, false);
        toggleDisplay(markup, false);

        const wandSelect = document.getElementById("wandlength");
        wandSelect.disabled = false;

        if (!colourType) return resolve();

        toggleDisplay(detail, true);

        getBlindName(blindType).then(blindName => {
            const tubePromise = getTubeName(tubeType).then(tubeName => {
                if (tubeName === "Plantation") {
                    toggleDisplay(battenfront, true);
                }
            }).catch(err => {
                resolve();
            });

            if (blindName === "Complete Set") {
                toggleDisplay(mounting, true);
                toggleDisplay(fabrictype, true);
                toggleDisplay(fabriccolour, true);
                toggleDisplay(drop, true);
                toggleDisplay(tracktype, true);
                toggleDisplay(wandlength, true);
                toggleDisplay(layoutcode, true);
                toggleDisplay(panelqty, true);

                tubePromise.finally(() => resolve());
            } else if (blindName === "Track Only") {
                toggleDisplay(mounting, true);
                toggleDisplay(tracktype, true);
                toggleDisplay(wandlength, true);
                toggleDisplay(layoutcode, true);
                toggleDisplay(panelqty, true);

                wandSelect.value = "Custom";
                wandSelect.disabled = true;
                visibleWandLength(wandSelect.value);
                resolve();
            } else if (blindName === "Panel Only") {
                toggleDisplay(fabrictype, true);
                toggleDisplay(fabriccolour, true);
                toggleDisplay(drop, true);

                tubePromise.finally(() => resolve());
            } else {
                resolve();
            }
        }).catch(err => {
            resolve();
        });

        if (typeof priceAccess !== "undefined" && priceAccess) {
            toggleDisplay(markup, true);
        }
    });
}

function visibleWandLength(wandLength) {
    return new Promise((resolve, reject) => {
        let thisDiv = document.getElementById("divwandlengthvalue");
        thisDiv.style.display = "none";

        if (wandLength === "Custom") {
            thisDiv.style.display = "";
        }
        resolve();
    });
}

function visibleLayoutCustom(layout) {
    return new Promise((resolve, reject) => {
        let thisDiv = document.getElementById("divlayoutcustom");
        thisDiv.style.display = "none";

        if (layout === "S") {
            thisDiv.style.display = "";
        }
        resolve();
    });
}

function process() {
    toggleButtonState(true, "Processing...");

    const fields = [
        "blindtype", "tubetype", "colourtype", "qty", "room", "mounting",
        "fabrictype", "fabriccolour", "width", "drop", "tracktype", "wandlength", "wandlengthvalue",
        "layoutcode", "layoutcodecustom", "panelqty", "batten", "notes", "markup"
    ];

    const formData = {
        headerid: headerId,
        itemaction: itemAction,
        itemid: itemId,
        designid: designId,
        loginid: loginId
    };

    fields.forEach(id => {
        formData[id] = document.getElementById(id).value;
    });

    $.ajax({
        type: "POST",
        url: "Method.aspx/PanelGlideProcess",
        data: JSON.stringify({ data: formData }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            const result = response.d.trim();
            if (result === "Success") {
                setTimeout(() => {
                    $('#modalSuccess').modal('show');
                    startCountdown(3);
                }, 500);
            } else {
                isError(result);
                toggleButtonState(false, "Submit");
            }
        },
        error: function () {
            toggleButtonState(false, "Submit");
        }
    });
}

function toggleButtonState(disabled, text) {
    $("#submit")
        .prop("disabled", disabled)
        .css("pointer-events", disabled ? "none" : "auto")
        .text(text);

    $("#cancel").prop("disabled", disabled).css("pointer-events", disabled ? "none" : "auto");
}

function startCountdown(seconds) {
    let countdown = seconds;
    const button = $("#vieworder");

    function updateButton() {
        button.text(`View Order (${countdown}s)`);
        countdown--;

        if (countdown >= 0) {
            setTimeout(updateButton, 1000);
        } else {
            window.location.href = `/order/detail?orderid=${headerId}`;
        }
    }
    updateButton();
}

async function bindItemOrder(itemId, companyDetailId) {
    try {
        const response = await $.ajax({
            type: "POST",
            url: "Method.aspx/PanelDetail",
            data: JSON.stringify({ itemId, companyDetailId }),
            contentType: "application/json; charset=utf-8",
            dataType: "json"
        });

        const data = response.d;

        fillSelect("#blindtype", data.BlindTypes);
        fillSelect("#tubetype", data.TubeTypes);
        fillSelect("#colourtype", data.ColourTypes);
        fillSelect("#mounting", data.Mountings);
        fillSelect("#fabrictype", data.Fabrics);
        fillSelect("#fabriccolour", data.FabricColours);
        bindLayoutCode(data.ItemData.BlindType, data.ItemData.TrackType);
        bindPanelQty(data.ItemData.TrackType, data.ItemData.LayoutCode);

        let manual = [
            bindLayoutCode(data.ItemData.BlindType, data.ItemData.TrackType),
            bindPanelQty(data.ItemData.TrackType, data.ItemData.LayoutCode)
        ];
        await Promise.all(manual);

        setFormValues(data.ItemData);

        document.getElementById("divloader").style.display = "none";
        document.getElementById("divorder").style.display = "";

        visibleProduct(data.ItemData.BlindType);
        visibleDetail(data.ItemData.BlindType, data.ItemData.TubeType, data.ItemData.ProductId);
        visibleLayoutCustom(data.ItemData.LayoutCode);
        visibleWandLength(data.ItemData.WandLength);
    } catch (error) {
        document.getElementById("divloader").style.display = "none";
    }
}

function setFormValues(itemData) {
    const mapping = {
        blindtype: "BlindType",
        tubetype: "TubeType",
        colourtype: "ProductId",
        qty: "Qty",
        room: "Room",
        mounting: "Mounting",
        fabrictype: "FabricId",
        fabriccolour: "FabricColourId",
        width: "Width",
        drop: "Drop",
        tracktype: "TrackType",
        wandlength: "WandLength",
        wandlengthvalue: "WandLengthValue",
        layoutcode: "LayoutCode",
        layoutcodecustom: "LayoutCodeCustom",
        panelqty: "PanelQty",
        batten: "Batten",
        notes: "Notes",
        markup: "MarkUp"
    };

    Object.keys(mapping).forEach(id => {
        const el = document.getElementById(id);
        if (!el) return;

        let value = itemData[mapping[id]];
        if (id === "markup" && value === 0) value = "";
        el.value = value || "";
    });
    const maxLength = 1000;
    const notesLength = (itemData["Notes"] || "").length;
    $("#notescount").text(`${notesLength}/${maxLength}`);

    if (itemAction === "copy") {
        const resetFields = ["room", "notes"];
        resetFields.forEach(id => {
            const el = document.getElementById(id);
            if (el) el.value = "";
        });
        $("#notescount").text(`0/${maxLength}`);
    }
}

function fillSelect(selector, list, selected = null) {
    const el = document.querySelector(selector);
    el.innerHTML = "<option value=''></option>";
    list.forEach(item => {
        const opt = document.createElement("option");
        opt.value = item.Value;
        opt.textContent = item.Text;
        if (selected != null && selected == item.Value) opt.selected = true;
        el.appendChild(opt);
    });
}

function controlForm(status, isEditItem, isCopyItem) {
    if (isEditItem === undefined) {
        isEditItem = false;
    }
    if (isCopyItem === undefined) {
        isCopyItem = false;
    }

    document.getElementById("submit").style.display = status ? "none" : "";

    const inputs = [
        "blindtype", "tubetype", "colourtype", "qty", "room", "mounting",
        "fabrictype", "fabriccolour", "width", "drop", "tracktype", "wandlength", "wandlengthvalue",
        "layoutcode", "layoutcodecustom", "panelqty", "batten", "notes", "markup"
    ];

    inputs.forEach(id => {
        const inputElement = document.getElementById(id);
        if (inputElement) {
            if (isCopyItem) {
                inputElement.disabled = (id === "blindtype");
            } else if (isEditItem && (id === "qty" || id === "blindtype")) {
                inputElement.disabled = true;
            } else {
                inputElement.disabled = status;
            }
        }
    });
}

async function initPanelGlide() {
    const urlParams = new URLSearchParams(window.location.search);
    const sessionId = urlParams.get("boos");
    if (!sessionId) return redirectOrder();

    const response = await fetch("Method.aspx/StringData", {
        method: "POST",
        headers: { "Content-Type": "application/json; charset=utf-8" },
        body: JSON.stringify({ type: "OrderContext", dataId: sessionId })
    });

    const result = await response.json();
    if (!result?.d) return redirectOrder();

    const params = new URLSearchParams(result.d);

    itemAction = params.get("do");
    headerId = params.get("orderid");
    itemId = params.get("itemid");
    designId = params.get("dtype");
    loginId = params.get("uid");

    if (!headerId) return redirectOrder();

    if (!itemAction || !designId || !loginId || designId !== designIdOri) {
        return window.location.href = `/order/detail?orderid=${headerId}`;
    }

    await Promise.all([
        getOrderHeader(headerId),
        getDesignName(designId),
        getFormAction(itemAction),
        getCompanyOrder(headerId),
        getCompanyDetailOrder(headerId),
        getRoleAccess(loginId),
        getPriceAccess(loginId)
    ]);

    if (itemAction === "create") {
        await bindBlindType(designId);
        controlForm(false);
        visibleProduct("");
        visibleDetail("", "", "");
        loader(itemAction);
    } else if (["edit", "view", "copy"].includes(itemAction)) {
        await bindItemOrder(itemId, companyDetail);
        controlForm(
            itemAction === "view",
            itemAction === "edit",
            itemAction === "copy"
        );
    }
}

function showInfo(type) {
    let info;

    if (type === "Layout") {
        info = "Layout Information";
        info += "<br /><br />";
        info += "Click " + "<a href='https://bigblinds.ordersblindonline.com/assets/document/pglayout.pdf' target='_blank'>here</a>" + " for the layouts.";
    } else if (type === "Layout Custom") {
        info = "Layout Custom Information";
        info += "<br /><br />";
        info += "L for Left Panel<br/>R for Right Panel<br/>W for Wand<br/><br/>Example: LLWWRRR";
    } else if (type === "Wand Length") {
        info = "Wand Length Information";
        info += "<br /><br />";
        info += "- Standard";
        info += "<br />";
        info += "Our standard wand length is 2/3 from your drop & maximum wand length is 1000mm";
        info += "<br /><br />";
        info += "- Custom";
        info += "<br />";
        info += "Minimum custom wand length is 2 / 3 from your drop";
        info += "<br />";
        info += "Maximum custom wand length is 1000mm.";
    }
    document.getElementById("spanInfo").innerHTML = info;
}

function redirectOrder() {
    window.location.replace("/order");
}

document.getElementById("modalSuccess").addEventListener("hide.bs.modal", function () {
    document.activeElement.blur();
    document.body.focus();
});

document.getElementById("modalError").addEventListener("hide.bs.modal", function () {
    document.activeElement.blur();
    document.body.focus();
});

document.getElementById("modalInfo").addEventListener("hide.bs.modal", function () {
    document.activeElement.blur();
    document.body.focus();
});