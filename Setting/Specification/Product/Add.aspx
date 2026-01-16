<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Add.aspx.vb" Inherits="Setting_Specification_Product_Add" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Product Add" %>

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
                            <li class="breadcrumb-item"><a runat="server" href="~/setting/specification">Specification</a></li>
                            <li class="breadcrumb-item"><a runat="server" href="~/setting/specification/product">Product</a></li>
                            <li class="breadcrumb-item active" aria-current="page"><%: Page.Title %></li>
                        </ol>
                    </nav>
                </div>
            </div>
        </div>
    </div>

    <div class="page-content">
        <section class="row">
            <div class="col-lg-8 col-md-12 col-sm-12">
                <div class="card">
                    <div class="card-header">
                        <h4 class="card-title">Add Form</h4>
                    </div>

                    <div class="card-content">
                        <div class="card-body">
                            <div class="form form-vertical">
                                <div class="form-body">
                                    <div class="row mb-2">
                                        <div class="col-12 col-sm-12 col-lg-6 mb-2">
                                            <div class="form-group">
                                                <label class="form-label">Design Type</label>
                                                <asp:DropDownList runat="server" ID="ddlDesign" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlDesign_SelectedIndexChanged"></asp:DropDownList>
                                            </div>
                                        </div>
                                        <div class="col-12 col-sm-12 col-lg-6 mb-2">
                                            <div class="form-group">
                                                <label class="form-label">Blind Type</label>
                                                <asp:DropDownList runat="server" ID="ddlBlind" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlBlind_SelectedIndexChanged"></asp:DropDownList>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="row mb-2">
                                        <div class="col-12">
                                            <div class="form-group">
                                                <label class="form-label">Company Detail</label>
                                                <asp:ListBox runat="server" ID="lbCompanyDetail" CssClass="choices form-select multiple-remove" SelectionMode="Multiple"></asp:ListBox>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="row mb-3">
                                        <div class="col-12">
                                            <div class="form-group">
                                                <label class="form-label">Product Name</label>
                                                <asp:TextBox runat="server" ID="txtName" CssClass="form-control" placeholder="" autocomplete="off"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="row mb-3">
                                        <div class="col-12">
                                            <div class="form-group">
                                                <label class="form-label">Invoice Name</label>
                                                <asp:TextBox runat="server" ID="txtInvoiceName" CssClass="form-control" placeholder="Invoice Name" autocomplete="off"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="row mb-3">
                                        <div class="col-12 col-sm-12 col-lg-4 mb-2">
                                            <div class="form-group">
                                                <label class="form-label">Tube Type</label>
                                                <asp:DropDownList runat="server" ID="ddlTube" CssClass="form-select"></asp:DropDownList>
                                            </div>
                                            <asp:Button runat="server" ID="btnAddTube" CssClass="btn btn-sm btn-primary" Text="Add New" OnClick="btnAddTube_Click" />
                                        </div>
                                        <div class="col-12 col-sm-12 col-lg-4 mb-2">
                                            <div class="form-group">
                                                <label class="form-label">Control Type</label>
                                                <asp:DropDownList runat="server" ID="ddlControl" CssClass="form-select"></asp:DropDownList>
                                            </div>
                                            <asp:Button runat="server" ID="btnAddControl" CssClass="btn btn-sm btn-primary" Text="Add New" OnClick="btnAddControl_Click" />
                                        </div>
                                        <div class="col-12 col-sm-12 col-lg-4 mb-2">
                                            <div class="form-group">
                                                <label class="form-label">Colour Type</label>
                                                <asp:DropDownList runat="server" ID="ddlColour" CssClass="form-select"></asp:DropDownList>
                                            </div>
                                            <asp:Button runat="server" ID="btnAddColour" CssClass="btn btn-sm btn-primary" Text="Add New" OnClick="btnAddColour_Click" />
                                        </div>
                                    </div>

                                    <div class="row mb-3">
                                        <div class="col-12">
                                            <div class="form-group">
                                                <label class="form-label">Description</label>
                                                <asp:TextBox runat="server" TextMode="MultiLine" ID="txtDescription" Height="100px" CssClass="form-control" placeholder="Description ..." autocomplete="off" style="resize:none;"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="row mb-3">
                                        <div class="col-12 col-sm-12 col-lg-3">
                                            <div class="form-group">
                                                <label class="form-label">Active</label>
                                                <asp:DropDownList runat="server" ID="ddlActive" CssClass="form-select">
                                                    <asp:ListItem Value="1" Text="Yes"></asp:ListItem>
                                                    <asp:ListItem Value="0" Text="No"></asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                    </div>

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
                    </div>

                    <div class="card-footer text-center">
                        <asp:Button runat="server" ID="btnSubmit" CssClass="btn btn-primary" Text="Submit" OnClick="btnSubmit_Click" />
                        <asp:Button runat="server" ID="btnCancel" CssClass="btn btn-danger" Text="Cancel" OnClick="btnCancel_Click" />
                    </div>
                </div>
            </div>
        </section>
    </div>

    <div class="modal fade text-left" id="modalTube" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title">Add Tube Type</h4>
                </div>

                <div class="modal-body">
                    <div class="row mb-2">
                        <div class="col-12">
                            <div class="form-group">
                                <label class="form-label">Name</label>
                                <asp:TextBox runat="server" ID="txtTubeName" CssClass="form-control" placeholder="Name ..." autocomplete="off"></asp:TextBox>
                            </div>
                        </div>
                    </div>
            
                    <div class="row mb-2">
                        <div class="col-12">
                            <div class="form-group">
                                <label class="form-label">Description</label>
                                <asp:TextBox runat="server" TextMode="MultiLine" ID="txtTubeDescription" Height="100px" CssClass="form-control" placeholder="Description ..." autocomplete="off" style="resize:none;"></asp:TextBox>
                            </div>
                        </div>
                    </div>

                    <div class="row mb-2" runat="server" id="divErrorProcessTube">
                        <div class="col-12">
                            <div class="alert alert-danger">
                                <span runat="server" id="msgErrorProcessTube"></span>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnTube" CssClass="btn btn-primary" Text="Submit" OnClick="btnTube_Click" />
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade text-left" id="modalControl" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title">Add Control Type</h4>
                </div>

                <div class="modal-body">
                    <div class="row mb-2">
                        <div class="col-12">
                            <div class="form-group">
                                <label class="form-label">Name</label>
                                <asp:TextBox runat="server" ID="txtControlName" CssClass="form-control" placeholder="Name ..." autocomplete="off"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                    
                    <div class="row mb-2">
                        <div class="col-12">
                            <div class="form-group">
                                <label class="form-label">Description</label>
                                <asp:TextBox runat="server" TextMode="MultiLine" ID="txtControlDescription" Height="100px" CssClass="form-control" placeholder="Description ..." autocomplete="off" style="resize:none;"></asp:TextBox>
                            </div>
                        </div>
                    </div>

                    <div class="row mb-2" runat="server" id="divErrorProcessControl">
                        <div class="col-12">
                            <div class="alert alert-danger">
                                <span runat="server" id="msgErrorProcessControl"></span>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnControl" CssClass="btn btn-primary" Text="Submit" OnClick="btnControl_Click" />
                </div>
            </div>
        </div>
    </div>
    
    <div class="modal fade text-left" id="modalColour" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title">Add Colour Type</h4>
                </div>

                <div class="modal-body">
                    <div class="row mb-2">
                        <div class="col-12">
                            <div class="form-group">
                                <label class="form-label">Name</label>
                                <asp:TextBox runat="server" ID="txtColourName" CssClass="form-control" placeholder="Name ..." autocomplete="off"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                
                    <div class="row mb-2">
                        <div class="col-12">
                            <div class="form-group">
                                <label class="form-label">Description</label>
                                <asp:TextBox runat="server" TextMode="MultiLine" ID="txtColourDescription" Height="100px" CssClass="form-control" placeholder="Description ..." autocomplete="off" style="resize:none;"></asp:TextBox>
                            </div>
                        </div>
                    </div>

                    <div class="row mb-2" runat="server" id="divErrorProcessColour">
                        <div class="col-12">
                            <div class="alert alert-danger">
                                <span runat="server" id="msgErrorProcessColour"></span>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnSubmitColour" CssClass="btn btn-primary" Text="Submit" OnClick="btnColour_Click" />
                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        function showTube() {
            $("#modalTube").modal("show");
        }

        function showControl() {
            $("#modalControl").modal("show");
        }
        function showColour() {
            $("#modalColour").modal("show");
        }

        ["modalTube", "modalControl", "modalColour"].forEach(function (id) {
            document.getElementById(id).addEventListener("hide.bs.modal", function () {
                document.activeElement.blur();
                document.body.focus();
            });
        });

        window.history.replaceState(null, null, window.location.href);
    </script>
</asp:Content>