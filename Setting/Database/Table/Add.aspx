<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Add.aspx.vb" Inherits="Setting_Database_Table_Add" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Add Table" %>

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
                            <li class="breadcrumb-item"><a runat="server" href="~/setting/database">Database</a></li>
                            <li class="breadcrumb-item"><a runat="server" href="~/setting/database/table">Table</a></li>
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
                        <h4 class="card-title">Create Form</h4>
                    </div>
                    <div class="card-content">
                        <div class="card-body">
                            <div class="form form-vertical">
                                <div class="form-body">
                                    <div class="row mb-3 form-group">
                                        <div class="col-12">
                                            <label class="form-label">TABLE NAME</label>
                                            <asp:TextBox runat="server" ID="txtName" CssClass="form-control" placeholder="Table Name ..." autocomplete="off"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="row form-group">
                                        <div class="col-12">
                                            <label class="form-label">FIELDS</label>
                                            <div class="table-responsive">
                                                <asp:GridView runat="server" ID="gvList" CssClass="table table-bordered table-hover mb-0" AutoGenerateColumns="false" OnRowDataBound="gvList_RowDataBound">
                                                    <Columns>
                                                        <asp:TemplateField HeaderText="NAME">
                                                            <ItemTemplate>
                                                                <asp:TextBox runat="server" ID="txtFieldName" CssClass="form-control"></asp:TextBox>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="TYPE">
                                                            <ItemTemplate>
                                                                <asp:DropDownList runat="server" ID="ddlType" CssClass="form-select">
                                                                    <asp:ListItem>int</asp:ListItem>
                                                                    <asp:ListItem>nvarchar(100)</asp:ListItem>
                                                                    <asp:ListItem>nvarchar(255)</asp:ListItem>
                                                                    <asp:ListItem>nvarchar(max)</asp:ListItem>
                                                                    <asp:ListItem>datetime</asp:ListItem>
                                                                    <asp:ListItem>bit</asp:ListItem>
                                                                    <asp:ListItem>decimal(18,2)</asp:ListItem>
                                                                </asp:DropDownList>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="KEY" ItemStyle-HorizontalAlign="Center">
                                                            <ItemTemplate>
                                                                <asp:CheckBox runat="server" ID="chkKey" />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="NOT NULL" ItemStyle-HorizontalAlign="Center">
                                                            <ItemTemplate>
                                                                <asp:CheckBox runat="server" ID="chkNotNull" />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                    </Columns>
                                                </asp:GridView>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row form-group">
                                        <div class="col-12">
                                            <asp:Button ID="btnAddRow" runat="server" Text="Add Column" CssClass="btn btn-secondary btn-sm" OnClick="btnAddRow_Click" />
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
            <div class="col-12 col-sm-12 col-lg-5"></div>
        </section>
    </div>

    <script type="text/javascript">
        window.history.replaceState(null, null, window.location.href);
    </script>
</asp:Content>
