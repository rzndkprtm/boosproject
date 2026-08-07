<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Delete.aspx.vb" Inherits="Setting_Price_Group_Delete" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Delete Price Group" %>

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
                            <li class="breadcrumb-item"><a runat="server" href="~/setting/price/group">Group</a></li>
                            <li class="breadcrumb-item active" aria-current="page"><%: Page.Title %></li>
                        </ol>
                    </nav>
                </div>
            </div>
        </div>
    </div>
    <div class="page-content">
        <section class="row">
            <div class="col-12 col-sm-12 col-lg-6">
                <div class="card">
                    <div class="card-header">
                        <h4 class="card-title">Price Group Form</h4>
                    </div>
                    <div class="card-body">
                        <div class="form form-vertical">
                            <div class="form-body">
                                <div class="row">
                                    <div class="col-12 form-group">
                                        <label class="form-label">Name</label>
                                        <asp:TextBox runat="server" ID="txtName" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row mb-3">
                                    <div class="col-12 form-group">
                                        <label class="form-label">Reason</label>
                                        <asp:TextBox runat="server" TextMode="MultiLine" ID="txtDescription" Height="100px" CssClass="form-control" placeholder="Description ..." autocomplete="off" style="resize:none;"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-12 form-group">
                                        <label class="form-label">New Price Group</label>
                                        <asp:DropDownList runat="server" ID="ddlPriceGroup" CssClass="choices form-select"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-12 form-group">
                                        <div class="alert alert-light-primary color-primary"><i class="bi bi-star"></i> This is primary alert.</div>
                                    </div>
                                </div>
                                <div class="row mt-5" runat="server" id="divError">
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
        </section>
    </div>

    <div runat="server" visible="false">
        <asp:Label runat="server" ID="lblId"></asp:Label>
        <asp:Label runat="server" ID="lblType"></asp:Label>
    </div>

    <script type="text/javascript">
        window.history.replaceState(null, null, window.location.href);
    </script>
</asp:Content>