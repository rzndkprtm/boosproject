<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Add.aspx.vb" Inherits="Setting_Customer_Contact_Add" MasterPageFile="~/Site.master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Add Customer Contact" %>

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
                            <li class="breadcrumb-item"><a runat="server" href="~/setting/customer">Customer</a></li>
                            <li class="breadcrumb-item"><a runat="server" href="~/setting/customer/contact">Contact</a></li>
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
                        <h4 class="card-title">Contact Form</h4>
                    </div>
                    <div class="card-content">
                        <div class="card-body">
                            <div class="form form-vertical">
                                <div class="form-body">
                                    <div class="row mb-2">
                                        <div class="col-12 form-group">
                                            <label class="form-label">Account</label>
                                            <asp:DropDownList runat="server" ID="ddlCustomer" CssClass="choices form-select"></asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="row mb-2">
                                        <div class="col-12 form-group">
                                            <label class="form-label">Name</label>
                                            <asp:TextBox runat="server" ID="txtName" CssClass="form-control" placeholder="Name ..." autocomplete="off"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="row mb-2">
                                        <div class="col-6 form-group">
                                            <label class="form-label">Email</label>
                                            <asp:TextBox runat="server" ID="txtEmail" TextMode="Email" CssClass="form-control" placeholder="Email ..." autocomplete="off"></asp:TextBox>
                                        </div>
                                        <div class="col-6 form-group">
                                            <label class="form-label">Phone</label>
                                            <asp:TextBox runat="server" ID="txtPhone" CssClass="form-control" placeholder="Phone ..." autocomplete="off"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="row mb-2">
                                        <div class="col-12 form-group">
                                            <label class="form-label">Tags</label>
                                            <asp:ListBox runat="server" ID="lbTags" CssClass="choices form-select multiple-remove" SelectionMode="Multiple">
                                                 <asp:ListItem Value="Confirming" Text="Confirming"></asp:ListItem>
                                                 <asp:ListItem Value="Invoicing" Text="Invoicing"></asp:ListItem>
                                                 <asp:ListItem Value="Quoting" Text="Quoting"></asp:ListItem>
                                                 <asp:ListItem Value="Newsletter" Text="Newsletter"></asp:ListItem>
                                            </asp:ListBox>
                                        </div>
                                    </div>
                                    <div class="row mb-2">
                                        <div class="col-12 form-group">
                                            <label class="form-label">Note</label>
                                            <asp:TextBox runat="server" TextMode="MultiLine" ID="txtNote" CssClass="form-control" Height="100px" placeholder="Note ..." autocomplete="off" style="resize:none;"></asp:TextBox>
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
            <div class="col-12 col-sm-12 col-lg-5">
                <div class="card">
                    <div class="card-header">
                        <h4 class="card-title text-center">Information</h4>
                    </div>
                    <div class="card-content">
                        <div class="card-body">
                            <ul>
                                <li>If this is the first contact created for the customer, it will automatically be assigned as the primary contact.</li> <li>The available tags should be selected based on the type of information the system will send to the contact.</li> <li>Please ensure all information entered is accurate and valid, especially the email address.</li> 
                            </ul>
                        </div>
                    </div>
                </div>
            </div>
        </section>
    </div>

    <div runat="server" visible="false">
        <asp:Label runat="server" ID="lblCustomerId"></asp:Label>
        <asp:Label runat="server" ID="lblReturnPage"></asp:Label>
    </div>

    <script type="text/javascript">
        window.history.replaceState(null, null, window.location.href);
    </script>
</asp:Content>