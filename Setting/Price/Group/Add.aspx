<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Add.aspx.vb" Inherits="Setting_Price_Group_Add" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Add Price Group" %>

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
            <div class="col-12 col-sm-12 col-lg-7">
                <div class="card">
                    <div class="card-header">
                        <h4 class="card-title">Price Group Form</h4>
                    </div>
                    <div class="card-body">
                        <div class="form form-vertical">
                            <div class="form-body">
                                <div class="row mb-2">
                                    <div class="col-12 form-group">
                                        <label class="form-label">Name</label>
                                        <asp:TextBox runat="server" ID="txtName" CssClass="form-control" placeholder="Price Group Name ..." autocomplete="off"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row mb-2">
                                    <div class="col-12 col-sm-12 col-lg-4 form-group">
                                        <label class="form-label">Company</label>
                                        <asp:DropDownList runat="server" ID="ddlCompany" CssClass="form-select"></asp:DropDownList>
                                    </div>
                                    <div class="col-12 col-sm-12 col-lg-4 form-group">
                                        <label class="form-label">Type</label>
                                        <asp:DropDownList runat="server" ID="ddlType" CssClass="form-select">
                                            <asp:ListItem Value="" Text=""></asp:ListItem>
                                            <asp:ListItem Value="Blinds" Text="Blinds"></asp:ListItem>
                                            <asp:ListItem Value="Shutters" Text="Shutters"></asp:ListItem>
                                            <asp:ListItem Value="Doors" Text="Doors"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                    <div class="col-12 col-sm-12 col-lg-4 form-group">
                                        <label class="form-label">Master</label>
                                        <asp:DropDownList runat="server" ID="ddlMaster" CssClass="form-select">
                                            <asp:ListItem Value="" Text=""></asp:ListItem>
                                            <asp:ListItem Value="No" Text="No"></asp:ListItem>
                                            <asp:ListItem Value="Yes" Text="Yes"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="row mb-2">
                                    <div class="col-12 form-group">
                                        <label class="form-label">Description</label>
                                        <asp:TextBox runat="server" TextMode="MultiLine" ID="txtDescription" Height="100px" CssClass="form-control" placeholder="Description ..." autocomplete="off" style="resize:none;"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row mb-2">
                                    <div class="col-12 col-sm-12 col-lg-3 form-group">
                                        <label class="form-label">Status</label>
                                        <asp:DropDownList runat="server" ID="ddlStatus" CssClass="form-select">
                                            <asp:ListItem Value="Active" Text="Active"></asp:ListItem>
                                            <asp:ListItem Value="Inactive" Text="Inactive"></asp:ListItem>
                                        </asp:DropDownList>
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
                    <div class="card-footer text-center">
                        <asp:Button runat="server" ID="btnSubmit" CssClass="btn btn-primary" Text="Submit" OnClick="btnSubmit_Click" />
                        <asp:Button runat="server" ID="btnCancel" CssClass="btn btn-danger" Text="Cancel" OnClick="btnCancel_Click" />
                    </div>
                </div>
            </div>
            <div class="col-12 col-sm-12 col-lg-5">
                <div class="card">
                    <div class="card-header">
                        <h4 class="card-title text-center">Information</h4>
                    </div>
                    <div class="card-content">
                        <div class="card-body"></div>
                    </div>
                </div>
            </div>
        </section>
    </div>

    <div class="modal fade" id="modalProductGroup" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Product Group</h5>
                </div>
                <div class="modal-body">
                    <div class="table-responsive">
                        <asp:GridView runat="server" ID="gvProductGroup" CssClass="table table-bordered table-hover mb-0" AutoGenerateColumns="false" ShowHeaderWhenEmpty="true" EmptyDataText="DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center" DataKeyNames="Id">
                            <Columns>
                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <asp:CheckBox ID="chkSelect" runat="server" CssClass="form-check chkSelectItem" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="Id" HeaderText="ID" />
                                <asp:BoundField DataField="Name" HeaderText="Name" />
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>
                <div class="modal-footer ">
                    <asp:Button runat="server" ID="btnClose" CssClass="btn btn-light-secondary" Text="Close" OnClick="btnClose_Click" />
                    <asp:Button runat="server" ID="btnProductGroup" CssClass="btn btn-danger" Text="Submit" OnClick="btnProductGroup_Click" />
                </div>
            </div>
        </div>
    </div>

    <div runat="server" visible="false">
        <asp:Label runat="server" ID="lblId"></asp:Label>
        <asp:Label runat="server" ID="lblType"></asp:Label>
    </div>

    <script type="text/javascript">
        function showProductGroup() {
            $("#modalProductGroup").modal("show");
        }
        window.history.replaceState(null, null, window.location.href);
    </script>
</asp:Content>
