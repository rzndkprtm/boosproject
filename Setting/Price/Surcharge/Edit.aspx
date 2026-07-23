<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Edit.aspx.vb" Inherits="Setting_Price_Surcharge_Edit" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Price Surcharge Edit" %>

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
                            <li class="breadcrumb-item"><a runat="server" href="~/setting">Setting</a></li>
                            <li class="breadcrumb-item"><a runat="server" href="~/setting/price">Price</a></li>
                            <li class="breadcrumb-item"><a runat="server" href="~/setting/price/surcharge">Surcharge</a></li>
                            <li class="breadcrumb-item active" aria-current="page"><%: Page.Title %></li>
                        </ol>
                    </nav>
                </div>
            </div>
        </div>
    </div>
    <div class="page-content">
        <section class="row">
            <div class="col-12 col-sm-12 col-lg-8">
                <div class="card">
                    <div class="card-header">
                        <h4 class="card-title">Surcharge Form</h4>
                    </div>
                    <div class="card-body">
                        <div class="form form-vertical">
                            <div class="form-body">
                                <asp:UpdatePanel ID="updateData" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <div class="row mb-2">
                                            <div class="col-12 form-group">
                                                <label class="form-label">Surcharge Name</label>
                                                <asp:TextBox runat="server" ID="txtName" CssClass="form-control" placeholder="Surcharge Name ...." autocomplete="off"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="row mb-2">
                                            <div class="col-12 col-sm-12 col-lg-5 form-group">
                                                <label class="form-label">Design Type</label>
                                                <asp:DropDownList runat="server" ID="ddlDesign" CssClass="choices form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlDesign_SelectedIndexChanged"></asp:DropDownList>
                                            </div>
                                            <div class="col-12 col-sm-12 col-lg-7 form-group">
                                                <label class="form-label">Price Group</label>
                                                <asp:ListBox runat="server" ID="lbPriceGroup" CssClass="choices form-select multiple-remove" SelectionMode="Multiple"></asp:ListBox>
                                            </div>
                                        </div>
                                        <div class="row mb-2">
                                            <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                <label class="form-label">Formula Type</label>
                                                <asp:DropDownList runat="server" ID="ddlFormulaType" CssClass="choices form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlFormulaType_SelectedIndexChanged">
                                                    <asp:ListItem Value="" Text=""></asp:ListItem>
                                                    <asp:ListItem Value="Standard" Text="Standard"></asp:ListItem>
                                                    <asp:ListItem Value="Custom" Text="Custom"></asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                        <div class="row mt-3 mb-4" runat="server" id="divFormulaStandard">
                                            <div class="col-12 col-sm-12 col-lg-6 form-group">
                                                <label class="form-label">Formula Type</label>
                                                <asp:DropDownList runat="server" ID="ddlFormulaField" CssClass="choices form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlFormulaField_SelectedIndexChanged"></asp:DropDownList>
                                            </div>
                                            <div class="col-12 col-sm-12 col-lg-6 form-group">
                                                <label class="form-label">Formula Data</label>
                                                <asp:DropDownList runat="server" ID="ddlFormulaData" CssClass="choices form-select"></asp:DropDownList>
                                            </div>
                                            <br />
                                            <div class="col-12 col-sm-12 col-lg-6 form-group">
                                                <label class="form-label">Formula Type (ADD)</label>
                                                <asp:DropDownList runat="server" ID="ddlFormulaFieldB" CssClass="choices form-select"></asp:DropDownList>
                                            </div>
                                            <div class="col-12 col-sm-12 col-lg-6 form-group">
                                                <label class="form-label">Formula Data (ADD)</label>
                                                <asp:DropDownList runat="server" ID="ddlFormulaDataB" CssClass="choices form-select"></asp:DropDownList>
                                            </div>
                                        </div>
                                        <div class="row mb-2" runat="server" id="divFormulaCustom">
                                            <div class="col-12 form-group">
                                                <label class="form-label">Formula</label>
                                                <asp:TextBox runat="server" TextMode="MultiLine" ID="txtFormula" Height="150px" CssClass="form-control" placeholder="Formula ..." autocomplete="off" style="resize:none;"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="row mb-2">
                                            <div class="col-12 col-sm-12 col-lg-6 form-group">
                                                <label class="form-label">Buy Charge</label>
                                                <asp:TextBox runat="server" TextMode="MultiLine" ID="txtBuyCharge" Height="200px" CssClass="form-control" placeholder="Buy Charge ..." autocomplete="off" style="resize:none;"></asp:TextBox>
                                            </div>
                                            <div class="col-12 col-sm-12 col-lg-6 form-group">
                                                <label class="form-label">Sell Charge</label>
                                                <asp:TextBox runat="server" TextMode="MultiLine" ID="txtSellCharge" Height="200px" CssClass="form-control" placeholder="Sell Charge ..." autocomplete="off" style="resize:none;"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="row mb-2">
                                            <div class="col-12 form-group">
                                                <label>Description</label>
                                                <asp:TextBox runat="server" TextMode="MultiLine" ID="txtDescription" Height="100px" CssClass="form-control" placeholder="Description ..." autocomplete="off" style="resize:none;"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="row mb-2">
                                            <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                <label>Active</label>
                                                <asp:DropDownList runat="server" ID="ddlActive" CssClass="form-select">
                                                    <asp:ListItem Value="1" Text="Yes"></asp:ListItem>
                                                    <asp:ListItem Value="0" Text="No"></asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                                <div class="row" runat="server" id="divError">
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
                        <asp:Button runat="server" ID="btnSubmit" CssClass="btn btn-primary" Text="Submit" OnClick="btnSubmit_Click" />
                        <asp:Button runat="server" ID="btnCancel" CssClass="btn btn-danger" Text="Cancel" OnClick="btnCancel_Click" />
                    </div>
                </div>
            </div>
            <div class="col-12 col-sm-12 col-lg-4">
                <div class="card">
                    <div class="card-header">
                        <h4 class="card-title text-center">Information</h4>
                    </div>
                    <div class="card-body">
                        <b>- Buy & Sell Charge</b>
                        <br /><br />
                        <b>Custom Condition</b>
                        <br />
                        Validation is performed using the fields specified in your custom condition.
                        <br />
                        If a field is not supported, please contact the developer.
                        <br /><br />
                        Click <a href="javascript:void(0);" runat="server" data-bs-toggle="modal" data-bs-target="#modalField">here</a> to view the list of supported fields.
                    </div>
                </div>
            </div>
        </section>
    </div>

    <div class="modal fade" id="modalField" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header bg-info">
                    <h5 class="modal-title white">List Field</h5>
                </div>
                <div class="modal-body">
                    <div class="table-responsive">
                        <asp:GridView
                            ID="gvListField" runat="server" CssClass="table table-bordered table-hover mb-0" AutoGenerateColumns="False" ShowHeaderWhenEmpty="true" EmptyDataText="DATA NOT FOUND :)">
                            <Columns>
                                <asp:TemplateField HeaderText="No." ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <%# Container.DataItemIndex + 1 %>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="FieldNameValue" HeaderText="Field Name" />
                                <asp:TemplateField HeaderText="Action" ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtCopy" runat="server" Text='<%# Eval("FieldNameValue") %>' Style="position:absolute; left:-9999px;" ReadOnly="true"></asp:TextBox>
                                        <button type="button" class="btn btn-sm btn-primary" onclick="copyRow(this)" data-bs-dismiss="modal"></button>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Close</a>
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

    <div runat="server" visible="false">
        <asp:Label runat="server" ID="lblId"></asp:Label>
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
        function copyRow(btn) {
            var row = btn.closest("tr");
            var txt = row.querySelector("input[type='text']");

            if (!txt) {
                alert("Textbox tidak ditemukan.");
                return;
            }

            txt.focus();
            txt.select();
            txt.setSelectionRange(0, 99999);

            var copied = document.execCommand("copy");

            if (copied) {
                var old = btn.innerHTML;
                btn.innerHTML = '<i class="fa fa-check"></i> Copied';

                setTimeout(function () {
                    btn.innerHTML = old;
                }, 1500);
            } else {
                alert("Copy gagal.");
            }
        }
        document.addEventListener("DOMContentLoaded", function () {
            initUpdatePanelLoading();
            initChoices();
        });
        ["modalField"].forEach(function (id) {
            document.getElementById(id).addEventListener("hide.bs.modal", function () {
                document.activeElement.blur();
                document.body.focus();
            });
        });
        window.history.replaceState(null, null, window.location.href);
    </script>
</asp:Content>
