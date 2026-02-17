let designIdOri = "12";
let itemAction;
let headerId;
let orderId;
let itemId;
let designId;
let customerId;
let companyId;
let companyDetailId;
let loginId;
let roleAccess;
let priceAccess;

initHorizon();

$("#submit").on("click", process);
$("#cancel").on("click", () => window.location.href = `/order/detail?orderid=${headerId}`);
$("#vieworder").on("click", () => window.location.href = `/order/detail?orderid=${headerId}`);

$("#blindtype").on("change", function () {
    bindTubeType($(this).val());
    bindMounting($(this).val());
});

$("#tubetype").on("change", function () {
    const blindtype = document.getElementById("blindtype").value;

    bindControlType(blindtype, $(this).val());
    bindFabricType(designId);
});

$("#controltype").on("change", function () {
    const blindtype = document.getElementById("blindtype").value;
    const tubetype = document.getElementById("tubetype").value;

    bindColourType(blindtype, tubetype, $(this).val());
    bindChain(designId, blindtype, $(this).val());
});

$("#colourtype").on("change", function () {
    const blindtype = document.getElementById("blindtype").value;
    const tubetype = document.getElementById("tubetype").value;
    const controltype = document.getElementById("controltype").value;

    visibleDetail(blindtype, tubetype, controltype, $(this).val());
});

$("#fabrictype").on("change", function () {
    bindFabricColour($(this).val());
});

$("#bottomtype").on("change", function () {
    bindBottomColour($(this).val());
});

$("#chaincolour").on("change", function () {
    const controllength = document.getElementById("controllength").value;

    bindChainStopper($(this).val());
});

$("#controllength").on("change", function () {
    visibleCustomChainLength($(this).val());
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
                orderId = d.OrderId || "-";
                customerId = d.CustomerId || "-";
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
            create: "Add Item", edit: "Edit Item",
            view: "View Item", copy: "Copy Item"
        };

        pageAction.innerText = actionMap[itemAction];
        resolve();
    });
}

function getCompanyOrder(headerId) {
    return new Promise((resolve, reject) => {
        companyId = "";

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
                companyId = response.d.trim();
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
        companyDetailId = "";

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
                companyDetailId = response.d.trim();
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

function getCompanyDetailName(companyDetailId) {
    if (!companyDetailId) return;

    const type = "CompanyDetailName";
    return new Promise((resolve, reject) => {
        $.ajax({
            type: "POST",
            url: "Method.aspx/StringData",
            data: JSON.stringify({ type: type, dataId: companyDetailId }),
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

function getControlName(controlType) {
    if (!controlType) return;

    const type = "ControlName";
    return new Promise((resolve, reject) => {
        $.ajax({
            type: "POST",
            url: "Method.aspx/StringData",
            data: JSON.stringify({ type: type, dataId: controlType }),
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

function getBottomName(bottomType) {
    if (!bottomType) return Promise.resolve("");

    const type = "BottomName";
    return new Promise((resolve, reject) => {
        $.ajax({
            type: "POST",
            url: "Method.aspx/StringData",
            data: JSON.stringify({ type: type, dataId: bottomType }),
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

function getChainLength(chainColour) {
    if (!chainColour) return;

    const type = "ChainLength";
    return new Promise((resolve, reject) => {
        $.ajax({
            type: "POST",
            url: "Method.aspx/StringData",
            data: JSON.stringify({ type: type, dataId: chainColour }),
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

function bindBlindType(designType) {
    return new Promise((resolve, reject) => {
        const blindtype = document.getElementById("blindtype");
        blindtype.innerHTML = "";

        if (!designType) {
            const selectedValue = blindtype.value || "";
            Promise.all([
                bindMounting(selectedValue),
                bindTubeType(selectedValue)
            ]).then(resolve).catch(reject);
            return;
        }

        const listData = { type: "BlindTypeRoller", companydetailid: companyDetailId, designtype: designType, action: itemAction };

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
                        bindMounting(selectedValue),
                        bindTubeType(selectedValue)
                    ]).then(resolve).catch(reject);
                } else {
                    const selectedValue = blindtype.value || "";
                    Promise.all([
                        bindMounting(selectedValue),
                        bindTubeType(selectedValue)
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
                bindControlType(blindType, selectedValue),
                bindFabricType(designId)
            ]).then(resolve).catch(reject);
            return;
        }

        let listData = { type: "TubeType", companydetailid: companyDetailId, blindtype: blindType, action: itemAction };

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
                        bindControlType(blindType, selectedValue),
                        bindFabricType(designId)
                    ]).then(resolve).catch(reject);
                } else {
                    const selectedValue = tubetype.value || "";
                    Promise.all([
                        bindControlType(blindType, selectedValue),
                        bindFabricType(designId)
                    ]).then(resolve).catch(reject);
                }
            },
            error: function (error) {
                reject(error);
            }
        });
    });
}

function bindControlType(blindType, tubeType) {
    return new Promise((resolve, reject) => {
        const controltype = document.getElementById("controltype");
        controltype.innerHTML = "";

        if (!blindType || !tubeType) {
            const selectedValue = controltype.value || "";
            Promise.all([
                bindColourType(blindType, tubeType, selectedValue),
                bindChain(designId, blindType, selectedValue)
            ]).then(resolve).catch(reject);
            return;
        }

        let listData = { type: "ControlType", companydetailid: companyDetailId, blindtype: blindType, tubetype: tubeType, action: itemAction };

        $.ajax({
            type: "POST",
            url: "Method.aspx/ListData",
            data: JSON.stringify({ data: listData }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (Array.isArray(response.d)) {
                    controltype.innerHTML = "";

                    if (response.d.length > 1) {
                        const defaultOption = document.createElement("option");
                        defaultOption.text = "";
                        defaultOption.value = "";
                        controltype.add(defaultOption);
                    }

                    response.d.forEach(function (item) {
                        const option = document.createElement("option");
                        option.value = item.Value;
                        option.text = item.Text;
                        controltype.add(option);
                    });

                    if (response.d.length === 1) {
                        controltype.selectedIndex = 0;
                    }

                    const selectedValue = controltype.value || "";
                    Promise.all([
                        bindColourType(blindType, tubeType, selectedValue),
                        bindChainRemote(designId, blindType, selectedValue)
                    ]).then(resolve).catch(reject);
                } else {
                    const selectedValue = controltype.value || "";
                    Promise.all([
                        bindColourType(blindType, tubeType, selectedValue),
                        bindChainRemote(designId, blindType, selectedValue)
                    ]).then(resolve).catch(reject);
                }
            },
            error: function (error) {
                reject(error);
            }
        });
    });
}

function bindColourType(blindType, tubeType, controlType) {
    return new Promise((resolve, reject) => {
        const colourtype = document.getElementById("colourtype");
        colourtype.innerHTML = "";

        if (!blindType || !tubeType || !controlType) {
            const selectedValue = colourtype.value || "";
            Promise.all([
                visibleDetail(blindType, tubeType, controlType, selectedValue)
            ]).then(resolve).catch(reject);
            return;
        }

        const listData = { type: "ColourType", companydetailid: companyDetailId, blindtype: blindType, tubetype: tubeType, controltype: controlType, action: itemAction };

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
                    Promise.all([
                        visibleDetail(blindType, tubeType, controlType, selectedValue)
                    ]).then(resolve).catch(reject);
                } else {
                    const selectedValue = colourtype.value || "";
                    Promise.all([
                        visibleDetail(blindType, tubeType, controlType, selectedValue)
                    ]).then(resolve).catch(reject);
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

        const listData = { type: "Mounting", blindtype: blindType, action: itemAction };

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

function bindChain(designType, blindType, controlType) {
    return new Promise((resolve, reject) => {
        const typeIds = ["chaincolour"];

        typeIds.forEach(id => {
            const select = document.getElementById(id);
            if (select) select.innerHTML = "";
        });

        if (!designType || !blindType || !controlType) {
            resolve();
            return;
        }

        let chainCustom = "";

        getBlindName(blindType).then(blindName => {
            if (blindName === "Full Cassette" || blindName === "Semi Cassette") {
                chainCustom = "Cassette";
            }

            const listData = { type: "ControlColour", designtype: designType, controltype: controlType, companydetailid: companyDetailId, customtype: chainCustom, action: itemAction };

            $.ajax({
                type: "POST",
                url: "Method.aspx/ListData",
                data: JSON.stringify({ data: listData }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    if (Array.isArray(response.d)) {
                        const hasMultiple = response.d.length > 1;

                        response.d.forEach((item, index) => {
                            const option = document.createElement("option");
                            option.value = item.Value;
                            option.text = item.Text;

                            typeIds.forEach(id => {
                                const select = document.getElementById(id);
                                if (select) {
                                    if (index === 0 && hasMultiple) {
                                        const defaultOption = document.createElement("option");
                                        defaultOption.text = "";
                                        defaultOption.value = "";
                                        select.add(defaultOption);
                                    }
                                    select.add(option.cloneNode(true));
                                }
                            });
                        });
                    }
                    resolve();
                },
                error: function (error) {
                    reject(error);
                }
            });
        }).catch(error => reject(error));
    });
}

function bindChainStopper(chainColour) {
    return new Promise((resolve, reject) => {
        const chainstopper = document.getElementById("chainstopper");
        chainstopper.innerHTML = "";

        if (!chainColour) {
            resolve();
            return;
        }

        const listData = { type: "ChainStopper", chaincolour: chainColour, action: itemAction };

        $.ajax({
            type: "POST",
            url: "Method.aspx/ListData",
            data: JSON.stringify({ data: listData }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (Array.isArray(response.d)) {
                    chainstopper.innerHTML = "";

                    if (response.d.length > 1) {
                        const defaultOption = document.createElement("option");
                        defaultOption.text = "";
                        defaultOption.value = "";
                        chainstopper.add(defaultOption);
                    }

                    response.d.forEach(function (item) {
                        const option = document.createElement("option");
                        option.value = item.Value;
                        option.text = item.Text;
                        chainstopper.add(option);
                    });

                    if (response.d.length === 1) {
                        chainstopper.selectedIndex = 0;
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

function bindFabricType(designType) {
    return new Promise((resolve, reject) => {
        const typeIds = ["fabrictype"];
        const bindFunctions = [bindFabricColour, bindFabricColourB, bindFabricColourC, bindFabricColourD, bindFabricColourE, bindFabricColourF];

        typeIds.forEach(id => {
            const select = document.getElementById(id);
            if (select) select.innerHTML = "";
        });

        if (!designType) {
            const bindPromises = typeIds.map((id, idx) => {
                const val = document.getElementById(id)?.value || "";
                return bindFunctions[idx](val);
            });
            Promise.all(bindPromises).then(resolve).catch(reject);
            return;
        }

        const listData = { type: "FabricTypeByDesign", designtype: designType, companydetailid: companyDetailId, action: itemAction };

        $.ajax({
            type: "POST",
            url: "Method.aspx/ListData",
            data: JSON.stringify({ data: listData }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (Array.isArray(response.d)) {
                    const hasMultiple = response.d.length > 1;

                    response.d.forEach((item, index) => {
                        const option = document.createElement("option");
                        option.value = item.Value;
                        option.text = item.Text;

                        typeIds.forEach(id => {
                            const select = document.getElementById(id);
                            if (select) {
                                if (index === 0 && hasMultiple) {
                                    const defaultOption = document.createElement("option");
                                    defaultOption.text = "";
                                    defaultOption.value = "";
                                    select.add(defaultOption);
                                }
                                select.add(option.cloneNode(true));
                            }
                        });
                    });

                    if (response.d.length === 1) {
                        typeIds.forEach(id => {
                            const select = document.getElementById(id);
                            if (select) select.selectedIndex = 0;
                        });
                    }

                    const bindPromises = typeIds.map((id, idx) => {
                        const val = document.getElementById(id)?.value || "";
                        return bindFunctions[idx](val);
                    });

                    Promise.all(bindPromises).then(resolve).catch(reject);
                } else {
                    const bindPromises = typeIds.map((id, idx) => {
                        const val = document.getElementById(id)?.value || "";
                        return bindFunctions[idx](val);
                    });
                    Promise.all(bindPromises).then(resolve).catch(reject);
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

        const listData = { type: "FabricColour", fabrictype: fabricType, action: itemAction };

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

function bindBottomType(designType) {
    return new Promise((resolve, reject) => {
        const typeIds = ["bottomtype"];
        const bindFunctions = [bindBottomColour, bindBottomColourB, bindBottomColourC, bindBottomColourD, bindBottomColourE, bindBottomColourF];
        const visibleFunctions = typeIds.map((_, i) => (val) => visibleFlatBottom(val, i + 1));

        typeIds.forEach(id => {
            const select = document.getElementById(id);
            if (select) select.innerHTML = "";
        });

        const callBottomVisibility = (idx, val) => {
            const blindNumber = idx + 1;
            return Promise.all([
                visibleFunctions[idx](val),
                visibleBottomColour(blindNumber, val)
            ]);
        };

        const callBindAndVisibility = (dataValue) => {
            return typeIds.map((id, idx) => {
                const val = document.getElementById(id)?.value || "";
                return bindFunctions[idx](dataValue, val)
                    .then(() => callBottomVisibility(idx, val));
            });
        };

        if (!designType) {
            Promise.all(callBindAndVisibility("")).then(resolve).catch(reject);
            return;
        }

        const listData = { type: "BottomType", designtype: designType, companydetailid: companyDetailId, action: itemAction };

        $.ajax({
            type: "POST",
            url: "Method.aspx/ListData",
            data: JSON.stringify({ data: listData }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (Array.isArray(response.d)) {
                    const hasMultiple = response.d.length > 1;

                    response.d.forEach((item, index) => {
                        const option = document.createElement("option");
                        option.value = item.Value;
                        option.text = item.Text;

                        typeIds.forEach(id => {
                            const select = document.getElementById(id);
                            if (select) {
                                if (index === 0 && hasMultiple) {
                                    const defaultOption = document.createElement("option");
                                    defaultOption.text = "";
                                    defaultOption.value = "";
                                    select.add(defaultOption);
                                }
                                select.add(option.cloneNode(true));
                            }
                        });
                    });

                    if (response.d.length === 1) {
                        typeIds.forEach(id => {
                            const select = document.getElementById(id);
                            if (select) select.selectedIndex = 0;
                        });
                    }

                    Promise.all(callBindAndVisibility(designType)).then(resolve).catch(reject);
                } else {
                    Promise.all(callBindAndVisibility(designType)).then(resolve).catch(reject);
                }
            },
            error: function (error) {
                reject(error);
            }
        });
    });
}

function bindBottomColour(bottomType) {
    return new Promise((resolve, reject) => {
        const bottomcolour = document.getElementById("bottomcolour");
        bottomcolour.innerHTML = "";

        if (!bottomType) {
            resolve();
            return;
        }

        let listData = { type: "BottomColour", bottomtype: bottomType, action: itemAction };

        $.ajax({
            type: "POST",
            url: "Method.aspx/ListData",
            data: JSON.stringify({ data: listData }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (Array.isArray(response.d)) {
                    bottomcolour.innerHTML = "";

                    if (response.d.length > 1) {
                        const defaultOption = document.createElement("option");
                        defaultOption.text = "";
                        defaultOption.value = "";
                        bottomcolour.add(defaultOption);
                    }

                    response.d.forEach(function (item) {
                        const option = document.createElement("option");
                        option.value = item.Value;
                        option.text = item.Text;
                        bottomcolour.add(option);
                    });

                    if (response.d.length === 1) {
                        bottomcolour.selectedIndex = 0;
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

function visibleDetail(blindType, tubeType, controlType, colourType) {
    return new Promise((resolve) => {
        const detail = document.getElementById("divdetail");
        const markup = document.getElementById("divmarkup");

        const divsToHide = [
            "divcontrollengthvalue", "divmarkup", "divprinting"
        ].map(id => document.getElementById(id));

        const toggleDisplay = (el, show) => {
            if (el) el.style.display = show ? "" : "none";
        };

        toggleDisplay(detail, false);
        toggleDisplay(markup, false);
        divsToHide.forEach(el => toggleDisplay(el, false));

        if (!blindType || !tubeType || !controlType || !colourType) return resolve();

        toggleDisplay(detail, true);

        resolve();
    });
}

function visibleCustomChainLength(chainLength) {
    return new Promise((resolve) => {
        let thisDiv = document.getElementById("divcontrollengthvalue");

        if (!thisDiv) {
            return resolve();
        }
        thisDiv.style.display = "none";

        if (chainLength === "Custom") {
            thisDiv.style.display = "";
            resolve();
        } else {
            resolve();
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
            setTimeout(updateButton, 500);
        } else {
            window.location.href = `/order/detail?orderid=${headerId}`;
        }
    }
    updateButton();
}

function controlForm(status, isEditItem, isCopyItem) {
    if (isEditItem === undefined) isEditItem = false;
    if (isCopyItem === undefined) isCopyItem = false;

    document.getElementById("submit").style.display = status ? "none" : "";

    const inputs = [
        "blindtype", "tubetype", "controltype", "colourtype", "qty", "room", "mounting",
        "fabrictype", "fabriccolour", "roll", "controlposition",
        "chaincolour", "chainstopper", "controllength", "controllengthvalue", "bottomtype", "bottomcolour",
        "width", "drop", "markup", "notes", "printing",
    ];

    inputs.forEach(id => {
        const el = document.getElementById(id);
        if (!el) return;

        if (isCopyItem) {
            el.disabled = (id === "blindtype");
            return;
        }

        if (isEditItem) {
            if (id === "qty" || id === "blindtype") {
                el.disabled = true;
            } else {
                el.disabled = false;
            }
            return;
        }

        el.disabled = status;
    });
}

function setFormValues(itemData) {
    const mapping = {
        blindtype: "BlindType",
        tubetype: "TubeType",
        controltype: "ControlType",
        colourtype: "ProductId",
        qty: "Qty",
        room: "Room",
        mounting: "Mounting",
        fabrictype: "FabricId",
        fabriccolour: "FabricColourId",
        roll: "Roll",
        controlposition: "ControlPosition",
        
        chaincolour: "ChainId",
        chainstopper: "ChainStopper",
        controllength: "ControlLength",
        controllengthvalue: "ControlLengthValue",
        bottomtype: "BottomId",
        bottomcolour: "BottomColourId",        
        width: "Width",
        drop: "Drop",       
        notes: "Notes",
        markup: "MarkUp",
        printing: "Printing"
    };

    Object.keys(mapping).forEach(id => {
        const el = document.getElementById(id);
        if (!el) return;

        let value = itemData[mapping[id]];
        if (id === "markup" && value === 0) value = "";
        el.value = value || "";
    });

    if (itemAction === "copy") {
        const resetFields = ["room", "notes"];
        resetFields.forEach(id => {
            const el = document.getElementById(id);
            if (el) el.value = "";
        });
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

function process() {
    toggleButtonState(true, "Processing...");

    const fields = [
        "blindtype", "tubetype", "controltype", "colourtype", "qty", "room", "mounting", "fabrictype", "fabriccolour", "roll", "controlposition", "chaincolour", "controllength", "bottomtype", "bottomcolour", "width", "drop", "markup", "notes", "printing"
    ];

    const formData = {
        headerid: headerId,
        orderid: orderId,
        itemaction: itemAction,
        itemid: itemId,
        designid: designId,
        loginid: loginId,
        rolename: roleAccess,
        customerid: customerId,
        companyid: companyId,
        companydetailid: companyDetailId
    };

    fields.forEach(id => {
        formData[id] = document.getElementById(id).value;
    });

    $.ajax({
        type: "POST",
        url: "Method.aspx/HorizonProcess",
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

async function initRoller() {
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

    updateLinkDetail(headerId);

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
        visibleDetail("", "", "", "");
        controlForm(false);
        bindBlindType(designId);
        bindBottomType(designId);
        loader(itemAction)
    } else if (["edit", "view", "copy"].includes(itemAction)) {
        controlForm(
            itemAction === "view",
            itemAction === "edit",
            itemAction === "copy"
        );
        await bindItemOrder(itemId, companyDetailId, itemAction);
    }
}

async function bindItemOrder(itemId, companyDetailId, action) {
    try {
        document.getElementById("divloader").style.display = "";

        const response = await $.ajax({
            type: "POST",
            url: "Method.aspx/RollerDetail",
            data: JSON.stringify({ itemId, companyDetailId, action }),
            contentType: "application/json; charset=utf-8",
            dataType: "json"
        });

        const data = response.d;

        fillSelect("#blindtype", data.BlindTypes);
        fillSelect("#tubetype", data.TubeTypes);
        fillSelect("#controltype", data.ControlTypes);
        fillSelect("#colourtype", data.ColourTypes);
        fillSelect("#mounting", data.Mountings);
        fillSelect("#fabrictype", data.Fabrics);
        fillSelect("#fabriccolour", data.FabricColours);
        fillSelect("#bottomtype", data.Bottoms);
        fillSelect("#bottomcolour", data.BottomColours);        
        fillSelect("#chaincolour", data.Chains);
        fillSelect("#chainstopper", data.Stoppers);

        setFormValues(data.ItemData);

        document.getElementById("divloader").style.display = "none";
        document.getElementById("divorder").style.display = "";

        visibleDetail(data.ItemData.BlindType, data.ItemData.TubeType, data.ItemData.ControlType, data.ItemData.ProductId);

        visibleCustomChainLength(data.ItemData.ControlLength);
    } catch (error) {
        document.getElementById("divloader").style.display = "none";
    }
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