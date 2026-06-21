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
            <div class="col-12 col-sm-12 col-lg-6 mb-2">
                <div class="card">
                    <div class="card-header">
                        <h4 class="card-title">Order Form</h4>
                    </div>
                    <div class="card-content">
                        <div class="card-body">
                            <div class="form form-vertical">
                                <div class="form-body">
                                    <div class="row mb-2" runat="server" id="divCustomer">
                                        <div class="col-12 form-group">
                                            <label class="form-label">Customer Account</label>
                                            <asp:DropDownList runat="server" ID="ddlCustomer" CssClass="choices form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlCustomer_SelectedIndexChanged"></asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="row mb-2" runat="server" id="divCreatedBy">
                                        <div class="col-12 form-group">
                                            <label class="form-label">Created By</label>
                                            <asp:DropDownList runat="server" ID="ddlCreatedBy" CssClass="choices form-select"></asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="row mb-2" runat="server" id="divMethod">
                                        <div class="col-12 col-sm-12 col-lg-5 form-group">
                                            <label class="form-label">Method</label>
                                            <asp:DropDownList runat="server" ID="ddlMethod" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlMethod_SelectedIndexChanged">
                                                <asp:ListItem Value="Manual" Text="Manual Entry"></asp:ListItem>
                                                <asp:ListItem Value="Upload" Text="Upload Excel"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                    <div runat="server" id="divManual">
                                        <div class="row mb-2">
                                            <div class="col-12 form-group">
                                                <label class="form-label">Order Number</label>
                                                <asp:TextBox runat="server" ID="txtOrderNumber" CssClass="form-control" placeholder="Order Number ..." autocomplete="off"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="row mb-2">
                                            <div class="col-12 form-group">
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
                                    </div>
                                    <div runat="server" id="divUpload">
                                        <div class="row">
                                            <div class="col-12 form-group">
                                                <label class="form-label">Upload Your File</label>
                                                <asp:FileUpload runat="server" ID="fuFile" CssClass="form-control" />
                                            </div>
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
                    </div>
                    <div class="card-footer text-center">
                        <asp:Button runat="server" ID="btnSubmit" CssClass="btn btn-primary" Text="Submit" OnClick="btnSubmit_Click" />
                        <asp:Button runat="server" ID="btnCancel" CssClass="btn btn-danger" Text="Cancel" OnClick="btnCancel_Click" />
                    </div>
                </div>
            </div>
            <div class="col-12 col-sm-12 col-lg-6">
                <div class="card">
                    <div class="card-header">
                        <h4 class="card-title text-center">Information</h4>
                    </div>
                    <div class="card-content">
                        <div class="card-body">

                        </div>
                    </div>
                </div>
            </div>
        </section>
    </div>

    <div class="modal modal-blur fade" id="modalInfo" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-header bg-info">
                    <h5 class="modal-title white">Information</h5>
                </div>
                <div class="modal-body">
                    <span id="spanInfo"></span>
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Close</a>
                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        window.history.replaceState(null, null, window.location.href);
    </script>
</asp:Content>
