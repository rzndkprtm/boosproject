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
                                        <div class="row" runat="server" visible="false">
                                            <div class="col-12 form-group">
                                                <label class="form-label">Order Address</label>
                                                <asp:TextBox runat="server" ID="txtOrderAddress" CssClass="form-control" ReadOnly="true" placeholder="Order Address ..." autocomplete="off" onclick="openOrderAddressModal();"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="row" runat="server" id="divOrderType">
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

    <div class="modal fade text-left" id="modalOrderAddress" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title">Order Address</h4>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <asp:TextBox runat="server" ID="txtShipmentOrderId" style="display:none;"></asp:TextBox>
                    <div class="row mb-2">
                        <div class="col-12 form-group">
                            <label class="form-label">Address</label>
                            <asp:TextBox runat="server" ID="txtAddress" CssClass="form-control" ClientIDMode="Static" placeholder="Address ..." autocomplete="off"></asp:TextBox>
                        </div>
                        <div class="col-12 form-group">
                            <label class="form-label">Suburb</label>
                            <asp:TextBox runat="server" ID="txtSuburb" CssClass="form-control" ClientIDMode="Static" placeholder="Suburb ..." autocomplete="off"></asp:TextBox>
                        </div>
                        <div class="col-12 form-group">
                            <label class="form-label">Suburb</label>
                            <asp:DropDownList runat="server" ID="ddlState" CssClass="form-select" ClientIDMode="Static">
                                <asp:ListItem Value="" Text=""></asp:ListItem>
                                <asp:ListItem Value="NSW" Text="NSW"></asp:ListItem>
                                <asp:ListItem Value="VIC" Text="VIC"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-12 form-group">
                            <label class="form-label">Post Code</label>
                            <asp:TextBox runat="server" ID="txtPostCode" CssClass="form-control" ClientIDMode="Static" placeholder="Post Code ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row mb-2" runat="server" id="divErrorShipmentOrder">
                        <div class="col-12">
                            <div class="alert alert-danger">
                                <span runat="server" id="msgErrorShipmentOrder"></span>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnShipmentOrder" CssClass="btn btn-primary" Text="Submit" OnClientClick="updateOrderAddress(); return false;" />
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
        function openOrderAddressModal() {
            var orderAddress = $('#<%= txtOrderAddress.ClientID %>').val().trim();

            $('#<%= txtAddress.ClientID %>').val('');
            $('#<%= txtSuburb.ClientID %>').val('');
            $('#<%= ddlState.ClientID %>').val('');
            $('#<%= txtPostCode.ClientID %>').val('');

            if (orderAddress !== '') {
                var parts = orderAddress.split(',').map(function (x) {
                    return x.trim();
                }).filter(function (x) {
                    return x !== '';
                });
                if (parts.length >= 3) {
                    var statePostCode = parts[parts.length - 1];
                    var match = statePostCode.match(
                        /^(NSW|VIC|QLD|SA|WA|TAS|NT|ACT)\s+(\d{4})$/i
                    );
                    if (match) {
                        var state = match[1].toUpperCase();
                        var postCode = match[2];
                        var suburb = parts[parts.length - 2];
                        var address = parts.slice(0, parts.length - 2).join(', ');

                        $('#<%= txtAddress.ClientID %>').val(address);
                        $('#<%= txtSuburb.ClientID %>').val(suburb);
                        $('#<%= ddlState.ClientID %>').val(state);
                        $('#<%= txtPostCode.ClientID %>').val(postCode);
                    } else {
                        $('#<%= txtAddress.ClientID %>').val(orderAddress);
                    }
                } else {
                    $('#<%= txtAddress.ClientID %>').val(orderAddress);
                }
            }

            var modalElement = document.getElementById('modalOrderAddress');
            var modal = new bootstrap.Modal(modalElement);
            modal.show();
        }

        function updateOrderAddress() {
            var address = $('#<%= txtAddress.ClientID %>').val().trim();
            var suburb = $('#<%= txtSuburb.ClientID %>').val().trim();
            var state = $('#<%= ddlState.ClientID %>').val().trim();
            var postCode = $('#<%= txtPostCode.ClientID %>').val().trim();

            var orderAddress = [address, suburb, state + ' ' + postCode].filter(function (x) {
                return x.trim() !== '';
            }).join(', ');

            $('#<%= txtOrderAddress.ClientID %>').val(orderAddress);

            var modalElement = document.getElementById('modalOrderAddress');
            var modal = bootstrap.Modal.getInstance(modalElement);

            if (modal) {
                modal.hide();
            }
        }

        ["modalOrderAddress"].forEach(function (id) {
            document.getElementById(id).addEventListener("hide.bs.modal", function () {
                document.activeElement.blur();
                document.body.focus();
            });
        });
        window.history.replaceState(null, null, window.location.href);
    </script>
</asp:Content>
