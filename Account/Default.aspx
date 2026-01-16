<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="Account_Default" MasterPageFile="~/Site.master" MaintainScrollPositionOnPostback="true" Debug="true" Title="My Account" %>

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
                            <li class="breadcrumb-item active" aria-current="page"><%: Page.Title %></li>
                        </ol>
                    </nav>
                </div>
            </div>
        </div>
    </div>

    <div class="page-content">
        <section class="row mb-3" runat="server" id="divError">
            <div class="col-12">
                <div class="alert alert-danger">
                    <span runat="server" id="msgError"></span>
                </div>
            </div>
        </section>
        <section class="row">
            <div class="col-12 col-sm-12 col-lg-6">
                <div class="card">
                    <div class="card-header">
                        <h4 class="card-title">Personal Account Information</h4>
                    </div>

                    <div class="card-content">
                        <div class="card-body">
                            <div class="row mb-3">
                                <div class="col-12 col-sm-12 col-lg-6 mb-3">
                                    <label>User Name</label>
                                    <br />
                                    <asp:Label runat="server" ID="lblUserName" CssClass="font-bold"></asp:Label>
                                </div>

                                <div class="col-12 col-sm-12 col-lg-6">
                                    <span>
                                        <label>Full Name</label>
                                        <a class="btn" href="javascript:void(0)" data-bs-toggle="modal" data-bs-target="#modalName"><i class="bi bi-pencil-square"></i></a>
                                    </span>
                                    <br />
                                    <asp:Label runat="server" ID="lblFullName" CssClass="font-bold"></asp:Label>
                                </div>
                            </div>

                            <div class="row mb-3">
                                <div class="col-12 col-sm-12 col-lg-6">
                                    <span>
                                        <label>Email</label>
                                        <a class="btn" href="javascript:void(0)" data-bs-toggle="modal" data-bs-target="#modalEmail"><i class="bi bi-pencil-square"></i></a>
                                    </span>                                    
                                    <br />
                                    <asp:Label runat="server" ID="lblUserEmail" CssClass="font-bold"></asp:Label>
                                </div>
                            </div>

                            <div class="row">
                                <div class="col-12">
                                    <ul class="list-group list-group-flush">
                                        <li class="list-group-item">
                                            This section is personal and applies only to the credentials you are currently using.
                                        </li>
                                        <li class="list-group-item"></li>
                                    </ul>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="col-12 col-sm-12 col-lg-6">
                <div class="card">
                    <div class="card-header">
                        <h4 class="card-title">Company Account Information</h4>
                    </div>

                    <div class="card-content">
                        <div class="card-body">
                            <div class="row mb-5">
                                <div class="col-9">
                                    <label>Account Name</label>
                                    <br />
                                    <asp:Label runat="server" ID="lblCustomerName" CssClass="font-bold"></asp:Label>
                                </div>
                                <div class="col-3">
                                    <label>Operator</label>
                                    <br />
                                    <asp:Label runat="server" ID="lblOperator" CssClass="font-bold"></asp:Label>
                                </div>
                            </div>

                            <div class="row mb-5">
                                <div class="col-12">
                                    <div class="table-responsive">
                                        <asp:GridView runat="server" ID="gvContact" CssClass="table table-bordered table-hover mb-0" AutoGenerateColumns="false" ShowHeaderWhenEmpty="true" EmptyDataText="DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center">
                                            <Columns>
                                                <asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-Width="60px">
                                                    <ItemTemplate>
                                                        <%# Container.DataItemIndex + 1 %>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="ContactName" HeaderText="Name" />
                                                <asp:BoundField DataField="Email" HeaderText="Email" />
                                                <asp:BoundField DataField="Tags" HeaderText="Tags" />
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>

                            <div class="row">
                                <div class="col-12">
                                    <ul class="list-group list-group-flush">
                                        <li class="list-group-item">
                                            This section cannot be modified by you. Please contact the IT or Accounting department for assistance.
                                        </li>
                                        <li class="list-group-item">
                                            This section contains our general information about your Account / Store / Retailer.
                                        </li>
                                        <li class="list-group-item"></li>
                                    </ul>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </section>
    </div>

    <div class="modal fade text-left" id="modalName" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title">Update Full Name</h4>
                </div>

                <div class="modal-body">
                    <div class="row mb-2">
                        <div class="col-12 form-group">
                            <label class="form-label">Full Name</label>
                            <asp:TextBox runat="server" ID="txtFullName" CssClass="form-control" placeholder="Full Name ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row mb-2" runat="server" id="divErrorName">
                        <div class="col-12">
                            <div class="alert alert-danger">
                                <span runat="server" id="msgErrorName"></span>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnName" CssClass="btn btn-primary" Text="Submit" OnClick="btnName_Click" />
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade text-left" id="modalEmail" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title">Update Personal Email</h4>
                </div>

                <div class="modal-body">
                    <div class="row mb-2">
                        <div class="col-12 form-group">
                            <label class="form-label">Email</label>
                            <asp:TextBox runat="server" ID="txtUserEmail" CssClass="form-control" placeholder="Email ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row mb-2" runat="server" id="divErrorEmail">
                        <div class="col-12">
                            <div class="alert alert-danger">
                                <span runat="server" id="msgErrorEmail"></span>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnEmail" CssClass="btn btn-primary" Text="Submit" OnClick="btnEmail_Click" />
                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        function showName() {
            $("#modalName").modal("show");
        }
        function showEmail() {
            $("#modalEmail").modal("show");
        }

        ["modalName", "modalEmail"].forEach(function (id) {
            document.getElementById(id).addEventListener("hide.bs.modal", function () {
                document.activeElement.blur();
                document.body.focus();
            });
        });

        window.history.replaceState(null, null, window.location.href);
    </script>
</asp:Content>