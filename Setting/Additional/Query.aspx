<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Query.aspx.vb" Inherits="Setting_Additional_Query" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Query" %>

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
                            <li class="breadcrumb-item"><a runat="server" href="~/setting/additional">Additional</a></li>
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
                        <h4 class="card-title">Query</h4>
                    </div>

                    <div class="card-content">
                        <div class="card-body">
                            <div class="form form-horizontal">
                                <div class="form-body">
                                    <div class="row mb-3">
                                        <div class="col-12 col-sm-12 col-lg-3">
                                            <label>Action Query</label>
                                        </div>
                                        <div class="col-12 col-sm-12 col-lg-4 form-group">
                                            <asp:DropDownList runat="server" ID="ddlAction" CssClass="form-select">
                                                <asp:ListItem Value="" Text=""></asp:ListItem>
                                                <asp:ListItem Value="Create" Text="Create"></asp:ListItem>
                                                <asp:ListItem Value="Read" Text="Read"></asp:ListItem>
                                                <asp:ListItem Value="Update" Text="Update"></asp:ListItem>
                                                <asp:ListItem Value="Delete" Text="Delete"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>

                                    <div class="row mb-3">
                                        <div class="col-12 col-sm-12 col-lg-3">
                                            <label>Your Query</label>
                                        </div>
                                        <div class="col-12 col-sm-12 col-lg-9 form-group">
                                            <asp:TextBox runat="server" ID="txtQuery" TextMode="MultiLine" Height="150px" CssClass="form-control" placeholder="Your Query ....." autocomplete="off" style="resize:none;"></asp:TextBox>
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

        <section class="row">
            <div class="col-12">
                <div class="card">
                    <div class="card-header">
                        <h4 class="card-title">Query Result</h4>
                    </div>

                    <div class="card-content">
                        <div class="card-body">
                            <div class="table-responsive">
                                <asp:GridView runat="server" ID="gvList" CssClass="table table-bordered table-hover" AutoGenerateColumns="true" EmptyDataText="DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center"></asp:GridView>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </section>
    </div>
</asp:Content>