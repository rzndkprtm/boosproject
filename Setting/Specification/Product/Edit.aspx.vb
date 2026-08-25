Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Specification_Product_Edit
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim url As String = String.Empty
    Dim dataLog As Object() = Nothing

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/specification/product", False)
            Exit Sub
        End If

        If String.IsNullOrEmpty(Request.QueryString("productid")) Then
            Response.Redirect("~/setting/specification/product", False)
            Exit Sub
        End If

        If Not String.IsNullOrEmpty(Request.QueryString("returnpage")) Then
            lblReturnPage.Text = Request.QueryString("returnpage").ToString()
        End If

        lblId.Text = Request.QueryString("productid").ToString()
        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindData(lblId.Text)
        End If
    End Sub

    Protected Sub ddlDesign_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindBlind(ddlDesign.SelectedValue)
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If ddlDesign.SelectedValue = "" Then
                MessageError(True, "DESIGN TYPE IS REQUIRED !")
                Exit Sub
            End If
            If ddlBlind.SelectedValue = "" Then
                MessageError(True, "BLIND TYPE IS REQUIRED !")
                Exit Sub
            End If
            Dim company As String = String.Empty
            For Each item As ListItem In lbCompanyDetail.Items
                If item.Selected Then
                    company += item.Value & ","
                End If
            Next
            If company = "" Then
                MessageError(True, "COMPANY IS REQUIRED !")
                Exit Sub
            End If
            'If ddlJobSheet.SelectedValue = "" Then
            '    MessageError(True, "JOB SHEET NAME IS REQUIRED !")
            '    Exit Sub
            'End If
            If txtName.Text = "" Then
                MessageError(True, "NAME IS REQUIRED !")
                Exit Sub
            End If
            If ddlControl.SelectedValue = "" Then
                MessageError(True, "CONTROL TYPE IS REQUIRED !")
                Exit Sub
            End If
            If ddlTube.SelectedValue = "" Then
                MessageError(True, "TUBE TYPE IS REQUIRED !")
                Exit Sub
            End If
            If ddlColour.SelectedValue = "" Then
                MessageError(True, "COLOUR TYPE IS REQUIRED !")
                Exit Sub
            End If
            If msgError.InnerText = "" Then
                Dim companyDetailId As String = company.Remove(company.Length - 1).ToString()
                Dim descText As String = txtDescription.Text.Replace(vbCrLf, "").Replace(vbCr, "").Replace(vbLf, "")
                If String.IsNullOrEmpty(txtInvoiceName.Text) Then txtInvoiceName.Text = txtName.Text

                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As SqlCommand = New SqlCommand("UPDATE Products SET DesignId=@DesignId, BlindId=@BlindId, CompanyDetailId=@CompanyDetailId, JobSheetId=@JobSheetId, Name=@Name, InvoiceName=@InvoiceName, TubeType=@TubeType, ControlType=@ControlType, ColourType=@ColourType, Description=@Description, Status=@Status WHERE Id=@Id", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", lblId.Text)
                        thisCmd.Parameters.AddWithValue("@DesignId", ddlDesign.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@BlindId", ddlBlind.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@CompanyDetailId", companyDetailId)
                        thisCmd.Parameters.AddWithValue("@JobSheetId", If(String.IsNullOrEmpty(ddlJobSheet.SelectedValue), CType(DBNull.Value, Object), ddlJobSheet.SelectedValue))
                        thisCmd.Parameters.AddWithValue("@Name", txtName.Text)
                        thisCmd.Parameters.AddWithValue("@InvoiceName", txtInvoiceName.Text)
                        thisCmd.Parameters.AddWithValue("@TubeType", ddlTube.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@ControlType", ddlControl.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@ColourType", ddlColour.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@Description", descText)
                        thisCmd.Parameters.AddWithValue("@Status", ddlStatus.SelectedValue)
                        thisConn.Open()
                        thisCmd.ExecuteNonQuery()
                    End Using
                End Using

                dataLog = {"Products", lblId.Text, Session("LoginId").ToString(), "Product Updated"}
                settingClass.Logs(dataLog)

                url = "~/setting/specification/product/"
                If lblReturnPage.Text = "detail" Then
                    url = String.Format("~/setting/specification/product/detail?productid={0}", lblId.Text)
                End If
                Response.Redirect(url, False)
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        url = "~/setting/specification/product/"
        If lblReturnPage.Text = "detail" Then
            url = String.Format("~/setting/specification/product/detail?productid={0}", lblId.Text)
        End If
        Response.Redirect(url, False)
    End Sub

    Protected Sub BindData(productId As String)
        Try
            Dim myData As DataRow = settingClass.GetDataRow("SELECT * FROM Products WHERE Id='" & productId & "' AND Status<>'Deleted'")
            If myData Is Nothing Then
                Response.Redirect("~/setting/specification/product/", False)
                Exit Sub
            End If

            Dim designId As String = myData("DesignId").ToString()
            Dim blindId As String = myData("BlindId").ToString()

            BindDesign()
            BindBlind(designId)
            BindCompanyDetail(blindId)
            BindJobSheet()
            BindControl()
            BindTube()
            BindColour()

            ddlDesign.SelectedValue = myData("DesignId").ToString()
            ddlBlind.SelectedValue = myData("BlindId").ToString()
            ddlJobSheet.SelectedValue = myData("JobSheetId").ToString()
            txtName.Text = myData("Name").ToString()
            txtInvoiceName.Text = myData("InvoiceName").ToString()
            ddlControl.SelectedValue = myData("ControlType").ToString()
            ddlTube.SelectedValue = myData("TubeType").ToString()
            ddlColour.SelectedValue = myData("ColourType").ToString()
            txtDescription.Text = myData("Description").ToString()
            ddlStatus.SelectedValue = myData("Status").ToString()

            If Not myData("CompanyDetailId").ToString() = "" Then
                Dim companyArray() As String = myData("CompanyDetailId").ToString().Split(",")
                For Each i In companyArray
                    If Not (i.Equals(String.Empty)) Then
                        lbCompanyDetail.Items.FindByValue(i).Selected = True
                    End If
                Next
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub BindDesign()
        ddlDesign.Items.Clear()
        Try
            ddlDesign.DataSource = settingClass.GetDataTable("SELECT * FROM Designs ORDER BY Name ASC")
            ddlDesign.DataTextField = "Name"
            ddlDesign.DataValueField = "Id"
            ddlDesign.DataBind()

            If ddlDesign.Items.Count > 1 Then
                ddlDesign.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub BindBlind(designId As String)
        ddlBlind.Items.Clear()
        Try
            If Not String.IsNullOrEmpty(designId) Then
                ddlBlind.DataSource = settingClass.GetDataTable("SELECT * FROM Blinds WHERE DesignId='" & designId & "' ORDER BY Name ASC")
                ddlBlind.DataTextField = "Name"
                ddlBlind.DataValueField = "Id"
                ddlBlind.DataBind()

                If ddlBlind.Items.Count > 1 Then
                    ddlBlind.Items.Insert(0, New ListItem("", ""))
                End If
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub BindCompanyDetail(blindId As String)
        lbCompanyDetail.Items.Clear()
        Try
            If Not String.IsNullOrEmpty(blindId) Then
                lbCompanyDetail.DataSource = settingClass.GetDataTable("SELECT CompanyDetails.* FROM Blinds CROSS APPLY STRING_SPLIT(Blinds.CompanyDetailId, ',') AS thisArray JOIN CompanyDetails ON CompanyDetails.Id=CAST(thisArray.value AS INT) WHERE Blinds.Id='" & blindId & "' AND (CompanyDetails.Status='Active' OR CompanyDetails.Status='Inactive') ORDER BY CompanyDetails.Name ASC;")
                lbCompanyDetail.DataTextField = "Name"
                lbCompanyDetail.DataValueField = "Id"
                lbCompanyDetail.DataBind()

                If lbCompanyDetail.Items.Count > 0 Then
                    lbCompanyDetail.Items.Insert(0, New ListItem("", ""))
                End If
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub BindJobSheet()
        ddlJobSheet.Items.Clear()
        Try
            ddlJobSheet.DataSource = settingClass.GetDataTable("SELECT * FROM JobSheets WHERE Status='Active' OR Status='Inactive' ORDER BY Name ASC")
            ddlJobSheet.DataTextField = "Name"
            ddlJobSheet.DataValueField = "Id"
            ddlJobSheet.DataBind()

            If ddlJobSheet.Items.Count > 0 Then
                ddlJobSheet.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            ddlJobSheet.Items.Clear()
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub BindTube()
        ddlTube.Items.Clear()
        Try
            ddlTube.DataSource = settingClass.GetDataTable("SELECT * FROM ProductTubes ORDER BY Name ASC")
            ddlTube.DataTextField = "Alias"
            ddlTube.DataValueField = "Id"
            ddlTube.DataBind()

            If ddlTube.Items.Count > 1 Then
                ddlTube.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub BindControl()
        ddlControl.Items.Clear()
        Try
            ddlControl.DataSource = settingClass.GetDataTable("SELECT * FROM ProductControls ORDER BY Name ASC")
            ddlControl.DataTextField = "Name"
            ddlControl.DataValueField = "Id"
            ddlControl.DataBind()

            If ddlControl.Items.Count > 1 Then
                ddlControl.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub BindColour()
        ddlColour.Items.Clear()
        Try
            ddlColour.DataSource = settingClass.GetDataTable("SELECT * FROM ProductColours ORDER BY Name ASC")
            ddlColour.DataTextField = "Name"
            ddlColour.DataValueField = "Id"
            ddlColour.DataBind()

            If ddlColour.Items.Count > 1 Then
                ddlColour.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerText = message
    End Sub

    Protected Function LoginAccess(action As String) As Boolean
        Try
            Dim roleId As String = Session("RoleId").ToString()
            Dim levelId As String = Session("LevelId").ToString()
            Dim accessClass As New AccessClass

            Return accessClass.GetLoginAccess(roleId, levelId, Page.Title, action)
        Catch ex As Exception
            Response.Redirect("~/account/login", False)
            HttpContext.Current.ApplicationInstance.CompleteRequest()
            Return False
        End Try
    End Function
End Class
