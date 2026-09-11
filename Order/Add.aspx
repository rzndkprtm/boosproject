<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Add.aspx.vb" Inherits="Order_Add" MasterPageFile="~/Site.master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Create Order" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-heading">
        <div class="page-title">
            <div class="row">
                <div class="col-12 col-md-6 order-md-1 order-last">
                    <h3><%: Page.Title %></h3>
                    <p class="text-subtitle text-muted"></p>
                </div>
                <div class="col-12 col-md-6 order-md-2 order-first">
                    <nav aria-label="breadcrumb" class="breadcrumb-header float-start float-lg-end">
                        <ol class="breadcrumb">
                            <li class="breadcrumb-item"><a runat="server" href="~/">Home</a></li>
                            <li class="breadcrumb-item"><a runat="server" href="~/order">Order</a></li>
                            <li class="breadcrumb-item active" aria-current="page"><%: Page.Title %></li>
                        </ol>
                    </nav>
                </div>
            </div>
        </div>
    </div>
    <div class="page-content">
        <section class="row">
            <div class="col-12 col-sm-12 col-lg-7 mb-2">
                <div class="card">
                    <div class="card-header">
                        <h4 class="card-title">Order Form</h4>
                    </div>
                    <div class="card-body">
                        <div class="form form-vertical">
                            <div class="form-body">
                                <asp:UpdatePanel ID="updateData" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <div class="row mb-2" runat="server" id="divCustomer">
                                            <div class="col-12 form-group">
                                                <label class="form-label">Customer Account</label>
                                                <asp:DropDownList runat="server" ID="ddlCustomer" CssClass="choices form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlCustomer_SelectedIndexChanged"></asp:DropDownList>
                                            </div>
                                        </div>
                                        <div class="row mb-2">
                                            <div class="col-12 col-sm-12 col-lg-5 form-group">
                                                <label class="form-label">Order Number</label>
                                                <asp:TextBox runat="server" ID="txtOrderNumber" CssClass="form-control" placeholder="Order Number ..." autocomplete="off"></asp:TextBox>
                                            </div>
                                            <div class="col-12 col-sm-12 col-lg-7 form-group">
                                                <label class="form-label">Order Name</label>
                                                <asp:TextBox runat="server" ID="txtOrderName" CssClass="form-control" placeholder="Order Name ..." autocomplete="off"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-12 form-group">
                                                <label class="form-label">Order Note</label>
                                                <asp:TextBox runat="server" TextMode="MultiLine" ID="txtOrderNote" Height="130px" CssClass="form-control" placeholder="Order Note ...." autocomplete="off" style="resize: none"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="row mb-2" runat="server" id="divOrderContact">
                                            <div class="col-12 form-group">
                                                <label class="form-label">Order Contact</label>
                                                <div class="input-group">
                                                    <asp:TextBox runat="server" ID="txtOrderContact" CssClass="form-control" ReadOnly="true" ClientIDMode="Static" placeholder="Order Contact ..." autocomplete="off" onclick="openOrderContactModal();"></asp:TextBox>
                                                    <span class="input-group-text" onclick="openOrderContactModal();">CLICK TO CHANGE</span>
                                                </div>
                                                <small class="text-muted">Format : {Contact Name} | {Phone} | {Email}</small>
                                                <asp:HiddenField runat="server" ID="hfOrderContact" ClientIDMode="Static" />
                                            </div>
                                        </div>
                                        <div class="row mb-2" runat="server" id="divOrderAddress">
                                            <div class="col-12 form-group">
                                                <label class="form-label">Order Address</label>
                                                <div class="input-group">
                                                    <asp:TextBox runat="server" ID="txtOrderAddress" CssClass="form-control" ReadOnly="true" ClientIDMode="Static" placeholder="Order Address ..." autocomplete="off" onclick="openOrderAddressModal();"></asp:TextBox>
                                                    <span class="input-group-text" onclick="openOrderAddressModal();">CLICK TO CHANGE</span>
                                                </div>
                                                <small class="text-muted">Format : {Address}, {Suburb}, {State} {Post Code}</small>
                                                <asp:HiddenField runat="server" ID="hfOrderAddress" ClientIDMode="Static" />
                                            </div>
                                        </div>
                                        <div class="row mb-2" runat="server" id="divOrderType">
                                            <div class="col-12 col-sm-12 col-lg-5 form-group">
                                                <label class="form-label">Order Type</label>
                                                <asp:DropDownList runat="server" ID="ddlOrderType" CssClass="form-select">
                                                    <asp:ListItem Value="Regular" Text="Regular"></asp:ListItem>
                                                    <asp:ListItem Value="Builder" Text="Builder"></asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                                <div class="row mt-3" runat="server" id="divError">
                                    <div class="col-12">
                                        <div class="alert alert-danger">
                                            <span runat="server" id="msgError"></span>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="card-footer text-center">
                        <asp:Button runat="server" ID="btnSubmit" CssClass="btn btn-primary" Text="Create Order" OnClick="btnSubmit_Click" />
                        <asp:Button runat="server" ID="btnCancel" CssClass="btn btn-danger" Text="Cancel" OnClick="btnCancel_Click" />
                    </div>
                </div>
            </div>
            <div class="col-12 col-sm-12 col-lg-5">
                <div class="card">
                    <div class="card-header">
                        <h4 class="card-title text-center">Information</h4>
                    </div>
                    <div class="card-body"></div>
                </div>
            </div>
        </section>
    </div>

    <div class="modal fade text-left" id="modalOrderContact" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title">Order Contact</h4>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <div class="row mb-2">
                        <div class="col-12 form-group">
                            <label class="form-label">Name</label>
                            <asp:TextBox runat="server" ID="txtContactName" CssClass="form-control" ClientIDMode="Static" placeholder="Address ..." autocomplete="off"></asp:TextBox>
                            <span id="spanErrorContactName" style="color:red;"></span>
                        </div>
                    </div>
                    <div class="row mb-2">
                        <div class="col-12 form-group">
                            <label class="form-label">Phone</label>
                            <asp:TextBox runat="server" ID="txtContactPhone" CssClass="form-control" ClientIDMode="Static" placeholder="Phone ..." autocomplete="off"></asp:TextBox>
                            <span id="spanErrorContactPhone" style="color:red;"></span>
                        </div>
                    </div>
                    <div class="row mb-2">
                        <div class="col-12 form-group">
                            <label class="form-label">Email</label>
                            <asp:TextBox runat="server" ID="txtContactEmail" CssClass="form-control" ClientIDMode="Static" placeholder="Email ..." autocomplete="off"></asp:TextBox>
                            <span id="spanErrorContactEmail" style="color:red;"></span>
                        </div>
                    </div>
                    <div class="row mt-3">
                        <div class="col-12">
                            <div class="alert alert-info">
                                Please contact Customer Service to set your contact as the default contact for all future orders.
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnOrderContact" CssClass="btn btn-primary" Text="Update" OnClientClick="updateOrderContact(); return false;" />
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade text-left" id="modalOrderAddress" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title">Order Address</h4>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <div class="row mb-2">
                        <div class="col-12 form-group">
                            <label class="form-label">Address</label>
                            <asp:TextBox runat="server" ID="txtAddress" CssClass="form-control" ClientIDMode="Static" placeholder="Address ..." autocomplete="off"></asp:TextBox>
                            <span id="spanErrorAddress" style="color:red;"></span>
                        </div>
                    </div>
                    <div class="row mb-2">
                        <div class="col-12 form-group">
                            <label class="form-label">Suburb</label>
                            <asp:TextBox runat="server" ID="txtSuburb" CssClass="form-control" ClientIDMode="Static" placeholder="Suburb ..." autocomplete="off"></asp:TextBox>
                            <span id="spanErrorSuburb" style="color:red;"></span>
                        </div>
                    </div>
                    <div class="row mb-2">
                        <div class="col-12 col-sm-12 col-lg-6 form-group">
                            <label class="form-label">State</label>
                            <asp:DropDownList runat="server" ID="ddlState" CssClass="form-select" ClientIDMode="Static">
                                <asp:ListItem Value="" Text=""></asp:ListItem>
                                <asp:ListItem Value="NSW" Text="NSW"></asp:ListItem>
                                <asp:ListItem Value="QLD" Text="QLD"></asp:ListItem>
                                <asp:ListItem Value="SA" Text="SA"></asp:ListItem>
                                <asp:ListItem Value="TAS" Text="TAS"></asp:ListItem>
                                <asp:ListItem Value="NT" Text="NT"></asp:ListItem>
                                <asp:ListItem Value="ACT" Text="ACT"></asp:ListItem>
                                <asp:ListItem Value="VIC" Text="VIC"></asp:ListItem>
                                <asp:ListItem Value="WA" Text="WA"></asp:ListItem>
                            </asp:DropDownList>
                            <span id="spanErrorState" style="color:red;"></span>
                        </div>
                        <div class="col-12 col-sm-12 col-lg-6 form-group">
                            <label class="form-label">Post Code</label>
                            <asp:TextBox runat="server" ID="txtPostCode" CssClass="form-control" ClientIDMode="Static" placeholder="Post Code ..." autocomplete="off"></asp:TextBox>
                            <span id="spanErrorPostCode" style="color:red;"></span>
                        </div>
                    </div>
                    <div class="row mt-3">
                        <div class="col-12">
                            <div class="alert alert-info">
                                Please contact Customer Service to set your address as the default address for all future orders.<br /><br />
                                If you use a different address, an additional delivery fee may apply to this order.
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnOrderAddress" CssClass="btn btn-primary" Text="Update" OnClientClick="updateOrderAddress(); return false;" />
                </div>
            </div>
        </div>
    </div>

    <div id="loadingOverlay" style="display:none; position:fixed; top:0; left:0; width:100%; height:100%; background:rgba(255,255,255,.5); z-index:99999;">
        <div class="position-absolute top-50 start-50 translate-middle">
            <div class="card shadow">
                <div class="card-body text-center">
                    <div class="spinner-border"></div>
                    <div class="mt-2">Loading...</div>
                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        window.addEventListener("pageshow", function () {
            var loading = document.getElementById("loadingOverlay");
            if (loading) loading.style.display = "none";
        });
        function initUpdatePanelLoading() {
            if (typeof Sys === "undefined") return;
            var prm = Sys.WebForms.PageRequestManager.getInstance();
            prm.add_beginRequest(function () {
                var loading = document.getElementById("loadingOverlay");
                if (loading) loading.style.display = "block";
            });
            prm.add_endRequest(function () {
                var loading = document.getElementById("loadingOverlay");
                if (loading) loading.style.display = "none";
                initChoices();
            });
        }
        function initChoices() {
            document.querySelectorAll("select.choices").forEach(function (el) {
                if (el.choices) {
                    el.choices.destroy();
                }
                el.choices = new Choices(el, {
                    searchEnabled: true,
                    itemSelectText: '',
                    shouldSort: false
                });
            });
        }
        document.addEventListener("DOMContentLoaded", function () {
            initUpdatePanelLoading();
            initChoices();
        });
        function openOrderContactModal() {
            var orderContact = $("#<%= txtOrderContact.ClientID %>").val().trim();

            $("#<%= txtContactName.ClientID %>").val("");
            $("#<%= txtContactPhone.ClientID %>").val("");
            $("#<%= txtContactEmail.ClientID %>").val("");

            if (orderContact !== "") {
                var parts = orderContact.split("|").map(function (x) {
                    return x.trim();
                });
                if (parts.length >= 1) {
                    $("#<%= txtContactName.ClientID %>").val(parts[0]);
                }
                if (parts.length >= 2) {
                    $("#<%= txtContactPhone.ClientID %>").val(parts[1]);
                }
                if (parts.length >= 3) {
                    $("#<%= txtContactEmail.ClientID %>").val(parts[2]);
                }
            }

            var modalElement = document.getElementById("modalOrderContact");
            var modal = new bootstrap.Modal(modalElement, {
                backdrop: "static",
                keyboard: false
            });
            modal.show();
        }
        function updateOrderContact() {
            $("#spanErrorContactName").text("");
            $("#spanErrorContactPhone").text("");
            $("#spanErrorContactEmail").text("");

            var contactName = $("#<%= txtContactName.ClientID %>").val().trim();
            var contactPhone = $("#<%= txtContactPhone.ClientID %>").val().trim();
            var contactEmail = $("#<%= txtContactEmail.ClientID %>").val().trim();

            var hasError = false;

            if (contactName === "") {
                $("#spanErrorContactName").text("CONTACT NAME IS REQUIRED !");
                hasError = true;
            }
            if (contactPhone === "") {
                $("#spanErrorContactPhone").text("CONTACT PHONE IS REQUIRED !");
                hasError = true;
            }
            if (contactEmail === "") {
                $("#spanErrorContactEmail").text("CONTACT EMAIL IS REQUIRED !");
                hasError = true;
            }
            if (hasError) {
                return false;
            }

            var orderContact = contactName + " | " + contactPhone + " | " + contactEmail;

            $("#txtOrderContact").val(orderContact);
            $("#hfOrderContact").val(orderContact);

            var modalElement = document.getElementById("modalOrderContact");
            var modal = bootstrap.Modal.getInstance(modalElement);

            if (modal) {
                modal.hide();
            }
            return false;
        }
        function openOrderAddressModal() {
            var orderAddress = $("#<%= txtOrderAddress.ClientID %>").val().trim();

            $("#<%= txtAddress.ClientID %>").val("");
            $("#<%= txtSuburb.ClientID %>").val("");
            $("#<%= ddlState.ClientID %>").val("");
            $("#<%= txtPostCode.ClientID %>").val("");

            if (orderAddress !== "") {
                var parts = orderAddress.split(",").map(function (x) {
                    return x.trim();
                }).filter(function (x) {
                    return x !== '';
                });

                if (parts.length >= 4) {
                    var postCode = parts[parts.length - 1];
                    var state = parts[parts.length - 2];
                    var suburb = parts[parts.length - 3];
                    var address = parts.slice(0, parts.length - 3).join(", ");

                    if (/^(NSW|VIC|QLD|SA|WA|TAS|NT|ACT)$/i.test(state) && /^\d{4,5}$/.test(postCode)) {
                        state = state.toUpperCase();

                        $("#<%= txtAddress.ClientID %>").val(address);
                        $("#<%= txtSuburb.ClientID %>").val(suburb);
                        $("#<%= ddlState.ClientID %>").val(state);
                        $("#<%= txtPostCode.ClientID %>").val(postCode);
                    } else {
                        $("#<%= txtAddress.ClientID %>").val(orderAddress);
                    }
                } else {
                    $("#<%= txtAddress.ClientID %>").val(orderAddress);
                }
            }

            var modalElement = document.getElementById("modalOrderAddress");
            var modal = new bootstrap.Modal(modalElement, {
                backdrop: "static",
                keyboard: false
            });

            modal.show();
        }
        function updateOrderAddress() {
            $("#spanErrorAddress").text("");
            $("#spanErrorSuburb").text("");
            $("#spanErrorState").text("");
            $("#spanErrorPostCode").text("");

            var address = $("#txtAddress").val().trim();
            var suburb = $("#txtSuburb").val().trim();
            var state = $("#ddlState").val().trim();
            var postCode = $("#txtPostCode").val().trim();

            var hasError = false;

            if (address === "") {
                $("#spanErrorAddress").text("ADDRESS IS REQUIRED !");
                hasError = true;
            }

            if (suburb === "") {
                $("#spanErrorSuburb").text("SUBURB IS REQUIRED !");
                hasError = true;
            }

            if (state === "") {
                $("#spanErrorState").text("STATE IS REQUIRED !");
                hasError = true;
            }

            if (postCode === "") {
                $("#spanErrorPostCode").text("POST CODE IS REQUIRED !");
                hasError = true;
            }
            else if (!/^\d{4,5}$/.test(postCode)) {
                $("#spanErrorPostCode").text("POST CODE MUST BE 4 OR 5 DIGITS !");
                hasError = true;
            }

            if (hasError) {
                return false;
            }

            var orderAddress = address + ", " + suburb + ", " + state + ", " + postCode;

            $("#txtOrderAddress").val(orderAddress);
            $("#hfOrderAddress").val(orderAddress);

            var modalElement = document.getElementById("modalOrderAddress");
            var modal = bootstrap.Modal.getInstance(modalElement);

            if (modal) {
                modal.hide();
            }

            return false;
        }
        ["modalOrderContact", "modalOrderAddress"].forEach(function (id) {
            document.getElementById(id).addEventListener("hide.bs.modal", function () {
                document.activeElement.blur();
                document.body.focus();
            });
        });
        window.history.replaceState(null, null, window.location.href);
    </script>
</asp:Content>
