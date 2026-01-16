<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Detail.aspx.vb" Inherits="Guide_Detail" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Guide Detail" %>

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
                            <li class="breadcrumb-item"><a runat="server" href="~/guide">Guide</a></li>
                            <li class="breadcrumb-item active" aria-current="page"><%: Page.Title %></li>
                        </ol>
                    </nav>
                </div>
            </div>
        </div>
    </div>

    <div class="page-content">
        <section class="row">
            <div class="col-12">
                <div class="row mb-2" runat="server" id="divError">
                    <div class="col-12">
                        <div class="alert alert-danger">
                            <span runat="server" id="msgError"></span>
                        </div>
                    </div>
                </div>
            </div>
        </section>
        <section class="row">
            <div class="col-12 col-sm-12 col-lg-9">
                <div class="card">
                    <div class="card-header">
                        <h4 class="card-title" runat="server" id="hTitle"></h4>
                    </div>
                    <div class="card-content">
                        <div class="card-body">
                            <div class="list-group list-group-horizontal-sm mb-1 text-center" id="dvTab" role="tablist">
                                <a class="list-group-item list-group-item-action active" id="listDescription" data-bs-toggle="list" href="#list-description" role="tab">Description</a>
                                <a class="list-group-item list-group-item-action" id="listVideo" data-bs-toggle="list" href="#list-video" role="tab">Video</a>
                                <a class="list-group-item list-group-item-action" id="listFile" data-bs-toggle="list" href="#list-file" role="tab">PDF</a>
                            </div>

                            <div class="tab-content text-justify">
                                <div class="tab-pane fade show active" id="list-description" role="tabpanel" aria-labelledby="listDescription">
                                    <div class="row mt-5">
                                        <div class="col-12">
                                            <span runat="server" id="spanDescription"></span>
                                        </div>
                                    </div>
                                </div>

                                <div class="tab-pane fade" id="list-video" role="tabpanel" aria-labelledby="listVideo">
                                    <div class="embed-responsive embed-responsive-item embed-responsive-16by9 w-100">
                                        <div class="row mt-3">
                                            <div class="col-12">
                                                <iframe runat="server" id="frmVideo" style="width:100%" height="500" allowfullscreen></iframe>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="tab-pane fade" id="list-file" role="tabpanel" aria-labelledby="listFile">
                                    <div class="row mt-3">
                                        <div class="col-12">
                                            <iframe runat="server" id="frmPdf" width="100%" height="1120" title="Reza"></iframe>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </section>
    </div>

    <div runat="server" visible="false">
        <asp:Label runat="server" ID="lblId"></asp:Label>
    </div>
</asp:Content>