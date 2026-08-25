Imports System.Data.SqlClient

Partial Class Setting_Specification_Product_Add
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim dataLog As Object() = Nothing

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/specification/product", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindDesign()
            BindBlind(ddlDesign.SelectedValue)
            BindCompanyDetail(ddlBlind.SelectedValue)
            BindJobSheet()
            BindControl()
            BindTube()
            BindColour()
        End If
    End Sub

    Protected Sub ddlDesign_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindBlind(ddlDesign.SelectedValue)
        BindCompanyDetail(ddlBlind.SelectedValue)
    End Sub

    Protected Sub ddlBlind_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindCompanyDetail(ddlBlind.SelectedValue)
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
                MessageError(True, "SUB COMPANY IS REQUIRED !")
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
                Dim thisId As String = settingClass.CreateId("SELECT TOP 1 Id FROM Products ORDER BY Id DESC")
                Dim companyDetailId As String = company.Remove(company.Length - 1).ToString()
                Dim descText As String = txtDescription.Text.Replace(vbCrLf, "").Replace(vbCr, "").Replace(vbLf, "")
                If String.IsNullOrEmpty(txtInvoiceName.Text) Then txtInvoiceName.Text = txtName.Text

                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As SqlCommand = New SqlCommand("INSERT INTO Products VALUES (@Id, @DesignId, @BlindId, @CompanyDetailId, @JobSheetId, @Name, @InvoiceName, @TubeType, @ControlType, @ColourType, @Description, @Status)", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", thisId)
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

                dataLog = {"Products", thisId, Session("LoginId").ToString(), "Product Created"}
                settingClass.Logs(dataLog)

                Dim url As String = String.Format("~/setting/specification/product/detail?productid={0}", thisId)
                Response.Redirect(url, False)
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/specification/product", False)
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
            ddlDesign.Items.Clear()
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
            ddlBlind.Items.Clear()
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub BindCompanyDetail(blindId As String)
        lbCompanyDetail.Items.Clear()
        Try
            If Not String.IsNullOrEmpty(blindId) Then
                lbCompanyDetail.DataSource = settingClass.GetDataTable("SELECT CompanyDetails.* FROM Blinds CROSS APPLY STRING_SPLIT(Blinds.CompanyDetailId, ',') AS thisArray JOIN CompanyDetails ON CompanyDetails.Id=CAST(thisArray.value AS INT) WHERE Blinds.Id='" & blindId & "' AND CompanyDetails.Status='Active' ORDER BY CompanyDetails.Name ASC;")
                lbCompanyDetail.DataTextField = "Name"
                lbCompanyDetail.DataValueField = "Id"
                lbCompanyDetail.DataBind()

                If lbCompanyDetail.Items.Count > 0 Then
                    lbCompanyDetail.Items.Insert(0, New ListItem("", ""))
                End If
            End If
        Catch ex As Exception
            lbCompanyDetail.Items.Clear()
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub BindJobSheet()
        ddlJobSheet.Items.Clear()
        Try
            ddlJobSheet.DataSource = settingClass.GetDataTable("SELECT * FROM JobSheets WHERE Status='Active' ORDER BY Name ASC")
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
            ddlTube.Items.Clear()
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
            ddlControl.Items.Clear()
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
            ddlColour.Items.Clear()
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

            Return AccessClass.GetLoginAccess(roleId, levelId, Page.Title, action)
        Catch ex As Exception
            Response.Redirect("~/account/login", False)
            HttpContext.Current.ApplicationInstance.CompleteRequest()
            Return False
        End Try
    End Function
End Class
