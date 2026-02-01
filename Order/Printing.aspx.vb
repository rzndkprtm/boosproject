Imports System.Data
Imports System.Data.SqlClient

Partial Class Order_Printing
    Inherits Page

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Dim orderClass As New OrderClass
    Dim mailingClass As New MailingClass

    Dim dataMailing As Object() = Nothing
    Dim dataLog As Object() = Nothing

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = PageAction("Load")
        If pageAccess = False Then
            Response.Redirect("~/order", False)
            Exit Sub
        End If

        If String.IsNullOrEmpty(Request.QueryString("boos")) Then
            Response.Redirect("~/order", False)
            Exit Sub
        End If

        lblId.Text = Request.QueryString("boos").ToString()
        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindData(lblId.Text)
        End If
    End Sub

    Protected Sub btnUpload_Click(sender As Object, e As EventArgs)
        Try
            Dim folderPath As String = Server.MapPath(String.Format("~/File/Order/{0}", lblOrderId.Text))
            If Not IO.Directory.Exists(folderPath) Then
                IO.Directory.CreateDirectory(folderPath)
            End If

            If Not fuUpload.HasFile Then
                MessageError(True, "FILE IS REQUIRED !")
                Exit Sub
            End If

            If Not String.IsNullOrEmpty(lblPrinting.Text) Then
                DeleteFile(lblPrinting.Text)
            End If

            Dim fileName As String = String.Format("Printing{0}{1}", lblItemId.Text, "1")
            Dim fileExt As String = IO.Path.GetExtension(fuUpload.FileName)
            Dim printing As String = String.Format("{0}{1}", fileName, fileExt)

            fuUpload.SaveAs(IO.Path.Combine(folderPath, printing))

            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As New SqlCommand("UPDATE OrderDetails SET Printing=@Printing WHERE Id=@Id", thisConn)
                    thisCmd.Parameters.AddWithValue("@Id", lblItemId.Text)
                    thisCmd.Parameters.AddWithValue("@Printing", printing)

                    thisConn.Open()
                    thisCmd.ExecuteNonQuery()
                End Using
            End Using

            orderClass.ResetPriceDetail(lblHeaderId.Text, lblItemId.Text)
            orderClass.CalculatePrice(lblHeaderId.Text, lblItemId.Text)
            orderClass.FinalCostItem(lblHeaderId.Text, lblItemId.Text)

            Dim dataLog As Object() = {"OrderDetails", lblItemId.Text, Session("LoginId"), "Update Printing Fabric"}
            orderClass.Logs(dataLog)

            Dim url As String = String.Format("~/order/printing?boos={0}", lblId.Text)
            Response.Redirect(url, False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnUploadB_Click(sender As Object, e As EventArgs)
        Try
            Dim folderPath As String = Server.MapPath(String.Format("~/File/Order/{0}", lblOrderId.Text))
            If Not IO.Directory.Exists(folderPath) Then
                IO.Directory.CreateDirectory(folderPath)
            End If

            If Not fuUploadB.HasFile Then
                MessageError(True, "FILE IS REQUIRED !")
                Exit Sub
            End If

            If Not String.IsNullOrEmpty(lblPrintingB.Text) Then
                DeleteFile(lblPrintingB.Text)
            End If

            Dim fileName As String = String.Format("Printing{0}{1}", lblItemId.Text, "2")
            Dim fileExt As String = IO.Path.GetExtension(fuUploadB.FileName)
            Dim printing As String = String.Format("{0}{1}", fileName, fileExt)

            fuUploadB.SaveAs(IO.Path.Combine(folderPath, printing))

            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As New SqlCommand("UPDATE OrderDetails SET PrintingB=@PrintingB WHERE Id=@Id", thisConn)
                    thisCmd.Parameters.AddWithValue("@Id", lblItemId.Text)
                    thisCmd.Parameters.AddWithValue("@PrintingB", printing)

                    thisConn.Open()
                    thisCmd.ExecuteNonQuery()
                End Using
            End Using

            orderClass.ResetPriceDetail(lblHeaderId.Text, lblItemId.Text)
            orderClass.CalculatePrice(lblHeaderId.Text, lblItemId.Text)
            orderClass.FinalCostItem(lblHeaderId.Text, lblItemId.Text)

            Dim dataLog As Object() = {"OrderDetails", lblItemId.Text, Session("LoginId"), "Update Printing Fabric"}
            orderClass.Logs(dataLog)

            Dim url As String = String.Format("~/order/printing?boos={0}", lblId.Text)
            Response.Redirect(url, False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnUploadC_Click(sender As Object, e As EventArgs)
        Try
            Dim folderPath As String = Server.MapPath(String.Format("~/File/Order/{0}", lblOrderId.Text))
            If Not IO.Directory.Exists(folderPath) Then
                IO.Directory.CreateDirectory(folderPath)
            End If

            If Not fuUploadC.HasFile Then
                MessageError(True, "FILE IS REQUIRED !")
                Exit Sub
            End If

            If Not String.IsNullOrEmpty(lblPrintingC.Text) Then
                DeleteFile(lblPrintingC.Text)
            End If

            Dim fileName As String = String.Format("Printing{0}{1}", lblItemId.Text, "3")
            Dim fileExt As String = IO.Path.GetExtension(fuUploadC.FileName)
            Dim printing As String = String.Format("{0}{1}", fileName, fileExt)

            fuUploadC.SaveAs(IO.Path.Combine(folderPath, printing))

            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As New SqlCommand("UPDATE OrderDetails SET PrintingC=@PrintingC WHERE Id=@Id", thisConn)
                    thisCmd.Parameters.AddWithValue("@Id", lblItemId.Text)
                    thisCmd.Parameters.AddWithValue("@PrintingC", printing)

                    thisConn.Open()
                    thisCmd.ExecuteNonQuery()
                End Using
            End Using

            orderClass.ResetPriceDetail(lblHeaderId.Text, lblItemId.Text)
            orderClass.CalculatePrice(lblHeaderId.Text, lblItemId.Text)
            orderClass.FinalCostItem(lblHeaderId.Text, lblItemId.Text)

            Dim dataLog As Object() = {"OrderDetails", lblItemId.Text, Session("LoginId"), "Update Printing Fabric"}
            orderClass.Logs(dataLog)

            Dim url As String = String.Format("~/order/printing?boos={0}", lblId.Text)
            Response.Redirect(url, False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnUploadD_Click(sender As Object, e As EventArgs)
        Try
            Dim folderPath As String = Server.MapPath(String.Format("~/File/Order/{0}", lblOrderId.Text))
            If Not IO.Directory.Exists(folderPath) Then
                IO.Directory.CreateDirectory(folderPath)
            End If

            If Not fuUploadD.HasFile Then
                MessageError(True, "FILE IS REQUIRED !")
                Exit Sub
            End If

            If Not String.IsNullOrEmpty(lblPrintingD.Text) Then
                DeleteFile(lblPrintingD.Text)
            End If

            Dim fileName As String = String.Format("Printing{0}{1}", lblItemId.Text, "4")
            Dim fileExt As String = IO.Path.GetExtension(fuUploadD.FileName)
            Dim printing As String = String.Format("{0}{1}", fileName, fileExt)

            fuUploadD.SaveAs(IO.Path.Combine(folderPath, printing))

            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As New SqlCommand("UPDATE OrderDetails SET PrintingD=@PrintingD WHERE Id=@Id", thisConn)
                    thisCmd.Parameters.AddWithValue("@Id", lblItemId.Text)
                    thisCmd.Parameters.AddWithValue("@PrintingD", printing)

                    thisConn.Open()
                    thisCmd.ExecuteNonQuery()
                End Using
            End Using

            orderClass.ResetPriceDetail(lblHeaderId.Text, lblItemId.Text)
            orderClass.CalculatePrice(lblHeaderId.Text, lblItemId.Text)
            orderClass.FinalCostItem(lblHeaderId.Text, lblItemId.Text)

            Dim dataLog As Object() = {"OrderDetails", lblItemId.Text, Session("LoginId"), "Update Printing Fabric"}
            orderClass.Logs(dataLog)

            Dim url As String = String.Format("~/order/printing?boos={0}", lblId.Text)
            Response.Redirect(url, False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnDelete_Click(sender As Object, e As EventArgs)
        Try
            DeleteFile(lblPrinting.Text)

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderDetails SET Printing=NULL WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", lblItemId.Text)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            orderClass.ResetPriceDetail(lblHeaderId.Text, lblItemId.Text)
            orderClass.CalculatePrice(lblHeaderId.Text, lblItemId.Text)
            orderClass.FinalCostItem(lblHeaderId.Text, lblItemId.Text)

            Dim dataLog As Object() = {"OrderDetails", lblItemId.Text, Session("LoginId"), "Delete Printing Fabric"}
            orderClass.Logs(dataLog)

            Dim url As String = String.Format("~/order/printing?boos={0}", lblId.Text)
            Response.Redirect(url, False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnDeleteB_Click(sender As Object, e As EventArgs)
        Try
            DeleteFile(lblPrintingB.Text)

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderDetails SET PrintingB=NULL WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", lblItemId.Text)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            orderClass.ResetPriceDetail(lblHeaderId.Text, lblItemId.Text)
            orderClass.CalculatePrice(lblHeaderId.Text, lblItemId.Text)
            orderClass.FinalCostItem(lblHeaderId.Text, lblItemId.Text)

            Dim dataLog As Object() = {"OrderDetails", lblItemId.Text, Session("LoginId"), "Delete Printing Fabric"}
            orderClass.Logs(dataLog)

            Dim url As String = String.Format("~/order/printing?boos={0}", lblId.Text)
            Response.Redirect(url, False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnDeleteC_Click(sender As Object, e As EventArgs)
        Try
            DeleteFile(lblPrintingC.Text)

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderDetails SET PrintingC=NULL WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", lblItemId.Text)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            orderClass.ResetPriceDetail(lblHeaderId.Text, lblItemId.Text)
            orderClass.CalculatePrice(lblHeaderId.Text, lblItemId.Text)
            orderClass.FinalCostItem(lblHeaderId.Text, lblItemId.Text)

            Dim dataLog As Object() = {"OrderDetails", lblItemId.Text, Session("LoginId"), "Delete Printing Fabric"}
            orderClass.Logs(dataLog)

            Dim url As String = String.Format("~/order/printing?boos={0}", lblId.Text)
            Response.Redirect(url, False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnDeleteD_Click(sender As Object, e As EventArgs)
        Try
            DeleteFile(lblPrintingD.Text)

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderDetails SET PrintingD=NULL WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", lblItemId.Text)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            orderClass.ResetPriceDetail(lblHeaderId.Text, lblItemId.Text)
            orderClass.CalculatePrice(lblHeaderId.Text, lblItemId.Text)
            orderClass.FinalCostItem(lblHeaderId.Text, lblItemId.Text)

            Dim dataLog As Object() = {"OrderDetails", lblItemId.Text, Session("LoginId"), "Delete Printing Fabric"}
            orderClass.Logs(dataLog)

            Dim url As String = String.Format("~/order/printing?boos={0}", lblId.Text)
            Response.Redirect(url, False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub BindData(id As String)
        Try
            Dim queryString As String = orderClass.GetItemData("SELECT QueryString FROM OrderActionContext WHERE Id='" & id & "'")
            If String.IsNullOrEmpty(queryString) Then
                Response.Redirect("~/order", False)
                Exit Sub
            End If

            Dim httpUtility = Web.HttpUtility.ParseQueryString(queryString)
            lblHeaderId.Text = Convert.ToString(httpUtility("headerid"))
            lblItemId.Text = Convert.ToString(httpUtility("itemid"))

            Dim headerData As DataRow = orderClass.GetDataRow("SELECT * FROM OrderHeaders WHERE Id='" & lblHeaderId.Text & "'")
            If headerData Is Nothing Then
                Response.Redirect("~/order", False)
                Exit Sub
            End If

            lblOrderId.Text = headerData("OrderId").ToString()
            lblOrderNumber.Text = headerData("OrderNumber").ToString()
            lblOrderName.Text = headerData("OrderName").ToString()
            lblStatus.Text = headerData("Status").ToString()

            Dim detailData As DataRow = orderClass.GetDataRow("SELECT OrderDetails.*, Products.Name AS ProductName, Designs.Name AS DesignName, Blinds.Name AS BlindName FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId=Products.Id LEFT JOIN Designs ON Products.DesignId=Designs.Id LEFT JOIN Blinds ON Products.BlindId=Blinds.Id WHERE OrderDetails.Id='" & lblItemId.Text & "' AND OrderDetails.Active=1")
            If detailData Is Nothing Then
                Response.Redirect("~/order/detail?orderid='" & lblHeaderId.Text & "'", False)
                Exit Sub
            End If

            descProduct.InnerHtml = String.Format("{0}, {1}", detailData("Room").ToString(), detailData("ProductName").ToString())

            Dim designName As String = detailData("DesignName").ToString()
            Dim blindName As String = detailData("BlindName").ToString()

            Dim width As String = detailData("Width").ToString()
            Dim widthB As String = detailData("WidthB").ToString()
            Dim widthC As String = detailData("WidthC").ToString()
            Dim widthD As String = detailData("WidthD").ToString()
            Dim widthE As String = detailData("WidthE").ToString()
            Dim widthF As String = detailData("WidthF").ToString()

            Dim drop As String = detailData("Drop").ToString()
            Dim dropB As String = detailData("DropB").ToString()
            Dim dropC As String = detailData("DropC").ToString()
            Dim dropD As String = detailData("DropD").ToString()
            Dim dropE As String = detailData("DropE").ToString()
            Dim dropF As String = detailData("DropF").ToString()

            Dim size As String = String.Format("({0}x{1})", width, drop)
            Dim sizeB As String = String.Format("({0}x{1})", widthB, dropB)
            Dim sizeC As String = String.Format("({0}x{1})", widthC, dropC)
            Dim sizeD As String = String.Format("({0}x{1})", widthD, dropD)
            Dim sizeE As String = String.Format("({0}x{1})", widthE, dropE)
            Dim sizeF As String = String.Format("({0}x{1})", widthF, dropF)

            Dim fabricColourId As String = detailData("FabricColourId").ToString()
            Dim fabricColourIdB As String = detailData("FabricColourIdB").ToString()
            Dim fabricColourIdC As String = detailData("FabricColourIdC").ToString()
            Dim fabricColourIdD As String = detailData("FabricColourIdD").ToString()
            Dim fabricColourIdE As String = detailData("FabricColourIdE").ToString()
            Dim fabricColourIdF As String = detailData("FabricColourIdF").ToString()

            Dim fabricColourName As String = orderClass.GetFabricColourName(fabricColourId)
            Dim fabricColourNameB As String = orderClass.GetFabricColourName(fabricColourIdB)
            Dim fabricColourNameC As String = orderClass.GetFabricColourName(fabricColourIdC)
            Dim fabricColourNameD As String = orderClass.GetFabricColourName(fabricColourIdD)
            Dim fabricColourNameE As String = orderClass.GetFabricColourName(fabricColourIdE)
            Dim fabricColourNameF As String = orderClass.GetFabricColourName(fabricColourIdF)

            Dim printing As String = detailData("Printing").ToString()
            Dim printingb As String = detailData("PrintingB").ToString()
            Dim printingc As String = detailData("PrintingC").ToString()
            Dim printingd As String = detailData("PrintingD").ToString()

            lblPrinting.Text = printing
            lblPrintingB.Text = printingb
            lblPrintingC.Text = printingc
            lblPrintingD.Text = printingd

            spanFabric.InnerText = String.Format("FABRIC : {0} {1}", fabricColourName.ToUpper(), size)
            spanFabricB.InnerText = String.Format("FABRIC : {0} {1}", fabricColourNameB.ToUpper(), sizeB)
            spanFabricC.InnerText = String.Format("FABRIC : {0} {1}", fabricColourNameC.ToUpper(), sizeC)
            spanFabricD.InnerText = String.Format("FABRIC : {0} {1}", fabricColourNameD.ToUpper(), sizeD)

            aPrinting.Visible = False : divPrinting.Visible = False : aUpload.Visible = False : aDelete.Visible = False
            aPrintingB.Visible = False : divPrintingB.Visible = False : aUploadB.Visible = False : aDeleteB.Visible = False
            aPrintingC.Visible = False : divPrintingC.Visible = False : aUploadC.Visible = False : aDeleteC.Visible = False
            aPrintingD.Visible = False : divPrintingD.Visible = False : aUploadD.Visible = False : aDeleteD.Visible = False

            If designName = "Roller Blind" Then
                If blindName = "Single Blind" Then
                    If width <= 1510 OrElse drop <= 1510 Then
                        aPrinting.Visible = True : divPrinting.Visible = True
                        aPrinting.Attributes("class") = aPrinting.Attributes("class") & " active"
                        divPrinting.Attributes("class") = divPrinting.Attributes("class") & " show active"

                        If lblStatus.Text = "Unsubmitted" Then aUpload.Visible = True

                        If Not String.IsNullOrEmpty(printing) Then
                            imgPrinting.ImageUrl = String.Format("~/File/Order/{0}/{1}", lblOrderId.Text, printing)
                            If lblStatus.Text = "Unsubmitted" Then aDelete.Visible = True
                        End If
                    End If
                End If

                If blindName = "Dual Blinds" Then
                    If width <= 1510 OrElse drop <= 1510 Then
                        aPrinting.Visible = True : divPrinting.Visible = True
                        aPrintingB.Visible = True : divPrintingB.Visible = True

                        aPrinting.Attributes("class") = aPrinting.Attributes("class") & " active"
                        divPrinting.Attributes("class") = divPrinting.Attributes("class") & " show active"

                        If lblStatus.Text = "Unsubmitted" Then
                            aUpload.Visible = True : aUploadB.Visible = True
                        End If

                        If Not String.IsNullOrEmpty(printing) Then
                            imgPrinting.ImageUrl = String.Format("~/File/Order/{0}/{1}", lblOrderId.Text, printing)
                            If lblStatus.Text = "Unsubmitted" Then aDelete.Visible = True
                        End If
                        If Not String.IsNullOrEmpty(printingb) Then
                            imgPrintingB.ImageUrl = String.Format("~/File/Order/{0}/{1}", lblOrderId.Text, printingb)
                            If lblStatus.Text = "Unsubmitted" Then aDeleteB.Visible = True
                        End If
                    End If
                End If

                If blindName = "Link 2 Blinds Dependent" OrElse blindName = "Link 2 Blinds Independent" Then
                    If width <= 1510 OrElse drop <= 1510 Then
                        aPrinting.Visible = True : divPrinting.Visible = True
                        If Not String.IsNullOrEmpty(printing) Then
                            imgPrinting.ImageUrl = String.Format("~/File/Order/{0}/{1}", lblOrderId.Text, printing)
                            If lblStatus.Text = "Unsubmitted" Then aDelete.Visible = True
                        End If
                    End If
                    If widthB <= 1510 OrElse dropB <= 1510 Then
                        aPrintingB.Visible = True : divPrintingB.Visible = True
                        If Not String.IsNullOrEmpty(printingb) Then
                            imgPrintingB.ImageUrl = String.Format("~/File/Order/{0}/{1}", lblOrderId.Text, printingb)
                            If lblStatus.Text = "Unsubmitted" Then aDeleteB.Visible = True
                        End If
                    End If

                    If lblStatus.Text = "Unsubmitted" Then aUpload.Visible = True : aUploadB.Visible = False

                    If aPrinting.Visible AndAlso divPrinting.Visible Then
                        aPrinting.Attributes("class") = (aPrinting.Attributes("class") & " active").Trim()
                        divPrinting.Attributes("class") = (divPrinting.Attributes("class") & " show active").Trim()
                    ElseIf aPrintingB.Visible AndAlso divPrintingB.Visible Then
                        aPrintingB.Attributes("class") = (aPrintingB.Attributes("class") & " active").Trim()
                        divPrintingB.Attributes("class") = (divPrintingB.Attributes("class") & " show active").Trim()
                    End If
                End If
                If blindName = "Link 3 Blinds Dependent" OrElse blindName = "Link 3 Blinds Independent with Dependent" Then
                    If width <= 1510 OrElse drop <= 1510 Then
                        aPrinting.Visible = True : divPrinting.Visible = True
                        If Not String.IsNullOrEmpty(printing) Then
                            imgPrinting.ImageUrl = String.Format("~/File/Order/{0}/{1}", lblOrderId.Text, printing)

                            If lblStatus.Text = "Unsubmitted" Then aDelete.Visible = True
                        End If
                    End If
                    If widthB <= 1510 OrElse dropB <= 1510 Then
                        aPrintingB.Visible = True : divPrintingB.Visible = True
                        If Not String.IsNullOrEmpty(printingb) Then
                            imgPrintingB.ImageUrl = String.Format("~/File/Order/{0}/{1}", lblOrderId.Text, printingb)
                        End If
                        If lblStatus.Text = "Unsubmitted" Then aDeleteB.Visible = True
                    End If
                    If widthC <= 1510 OrElse dropC <= 1510 Then
                        aPrintingC.Visible = True : divPrintingC.Visible = True
                        If Not String.IsNullOrEmpty(printingc) Then
                            imgPrintingC.ImageUrl = String.Format("~/File/Order/{0}/{1}", lblOrderId.Text, printingc)
                        End If
                        If lblStatus.Text = "Unsubmitted" Then aDeleteC.Visible = True
                    End If

                    If lblStatus.Text = "Unsubmitted" Then aUpload.Visible = True : aUploadB.Visible = False : aUploadC.Visible = False

                    If aPrinting.Visible AndAlso divPrinting.Visible Then
                        aPrinting.Attributes("class") = (aPrinting.Attributes("class") & " active").Trim()
                        divPrinting.Attributes("class") = (divPrinting.Attributes("class") & " show active").Trim()
                    ElseIf aPrintingB.Visible AndAlso divPrintingB.Visible Then
                        aPrintingB.Attributes("class") = (aPrintingB.Attributes("class") & " active").Trim()
                        divPrintingB.Attributes("class") = (divPrintingB.Attributes("class") & " show active").Trim()
                    ElseIf aPrintingC.Visible AndAlso divPrintingC.Visible Then
                        aPrintingC.Attributes("class") = (aPrintingC.Attributes("class") & " active").Trim()
                        divPrintingC.Attributes("class") = (divPrintingC.Attributes("class") & " show active").Trim()
                    End If
                End If
            End If

            If designName = "Roman Blind" Then
                If width <= 1510 OrElse drop <= 1510 Then
                    aPrinting.Visible = True : divPrinting.Visible = True
                    aPrinting.Attributes("class") = aPrinting.Attributes("class") & " active"
                    divPrinting.Attributes("class") = divPrinting.Attributes("class") & " show active"

                    If lblStatus.Text = "Unsubmitted" Then aUpload.Visible = True

                    If Not String.IsNullOrEmpty(printing) Then
                        imgPrinting.ImageUrl = String.Format("~/File/Order/{0}/{1}", lblOrderId.Text, printing)
                        If lblStatus.Text = "Unsubmitted" Then aDelete.Visible = True
                    End If
                End If
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                If Session("RoleName") = "Customer" Then
                    MessageError(True, "PLEASE CONTACT YOUR CUSTOMER SERVICE !")
                End If
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "BindData", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
        End Try
    End Sub

    Protected Sub DeleteFile(fileName As String)
        Try
            Dim folderPath As String = Server.MapPath(String.Format("~/File/Order/{0}", lblOrderId.Text))
            Dim filePath As String = IO.Path.Combine(folderPath, fileName)
            If IO.File.Exists(filePath) Then
                IO.File.Delete(filePath)
            End If
        Catch ex As Exception
        End Try
    End Sub

    Protected Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerText = message
    End Sub

    Protected Function PageAction(action As String) As Boolean
        Try
            Dim roleId As String = Session("RoleId").ToString()
            Dim levelId As String = Session("LevelId").ToString()
            Dim actionClass As New ActionClass

            Return actionClass.GetActionAccess(roleId, levelId, Page.Title, action)
        Catch ex As Exception
            Response.Redirect("~/account/login", False)
            HttpContext.Current.ApplicationInstance.CompleteRequest()
            Return False
        End Try
    End Function
End Class
