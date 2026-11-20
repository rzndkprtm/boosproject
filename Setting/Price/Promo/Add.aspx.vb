Imports System.Data.SqlClient

Partial Class Setting_Price_Promo_Add
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/price/promo", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindDataType(ddlType.SelectedValue)
            BindDataId(ddlDataType.SelectedValue)
        End If
    End Sub

    Protected Sub ddlType_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindDataType(ddlType.SelectedValue)
        BindDataId(ddlDataType.SelectedValue)
    End Sub

    Protected Sub ddlDataType_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindDataId(ddlDataType.SelectedValue)
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If ddlType.SelectedValue = "" Then
                MessageError(True, "PROMO TYPE IS REQUIRED !")
                Exit Sub
            End If
            If ddlDataType.SelectedValue = "" Then
                MessageError(True, "DATA TYPE IS REQUIRED !")
                Exit Sub
            End If
            If ddlDataId.SelectedValue = "" Then
                MessageError(True, "DATA NAME IS REQUIRED !")
                Exit Sub
            End If
            If txtName.Text = "" Then
                MessageError(True, "PROMO NAME IS REQUIRED !")
                Exit Sub
            End If
            If txtStartDate.Text = "" Then
                MessageError(True, "START DATE IS REQUIRED !")
                Exit Sub
            End If
            If txtEndDate.Text = "" Then
                MessageError(True, "END DATE IS REQUIRED !")
                Exit Sub
            End If
            Dim startDate As Date = Date.Parse(txtStartDate.Text)
            Dim endDate As Date = Date.Parse(txtEndDate.Text)

            If startDate > endDate Then
                MessageError(True, "START DATE MUST NOT BE LATER THAN END DATE !")
                Exit Sub
            End If

            If msgError.InnerText = "" Then
                Dim descText As String = txtDescription.Text.Replace(vbCrLf, "").Replace(vbCr, "").Replace(vbLf, "")

                Dim thisId As String = settingClass.CreateId("SELECT TOP 1 Id FROM Promos ORDER BY Id DESC")
                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As SqlCommand = New SqlCommand("INSERT INTO Promos VALUES (@Id, @Type, @DataType, @DataId, @Name, @StartDate, @EndDate, @Description, @Status)", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", thisId)
                        thisCmd.Parameters.AddWithValue("@Type", ddlType.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@DataType", ddlDataType.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@DataId", ddlDataId.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@Name", txtName.Text.Trim())
                        thisCmd.Parameters.AddWithValue("@StartDate", txtStartDate.Text)
                        thisCmd.Parameters.AddWithValue("@EndDate", txtEndDate.Text)
                        thisCmd.Parameters.AddWithValue("@Description", descText)
                        thisCmd.Parameters.AddWithValue("@Status", ddlStatus.SelectedValue)
                        thisConn.Open()
                        thisCmd.ExecuteNonQuery()
                    End Using
                End Using

                Dim dataLog As Object() = {"Promos", thisId, Session("LoginId").ToString(), "Promo Created"}
                settingClass.Logs(dataLog)

                Dim url As String = String.Format("~/setting/price/promo/detail?promoid={0}", thisId)
                Response.Redirect(url, False)
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/price/promo", False)
    End Sub

    Protected Sub BindDataType(type As String)
        ddlDataType.Items.Clear()
        Try
            If type = "Buy" Then
                ddlDataType.Items.Add(New ListItem("Companys", "Companys"))
                ddlDataType.Items.Add(New ListItem("Customers", "Customers"))
            End If
            If type = "Sell" OrElse type = "Factory" Then
                ddlDataType.Items.Add(New ListItem("Companys", "Companys"))
            End If

            If ddlDataType.Items.Count > 1 Then
                ddlDataType.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            ddlDataType.Items.Clear()
            If Session("RoleName") = "Developer" Then
                MessageError(True, ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub BindDataId(dataType As String)
        ddlDataId.Items.Clear()
        Try
            If Not String.IsNullOrEmpty(dataType) Then
                ddlDataId.DataSource = settingClass.GetDataTable("SELECT Id, Name FROM " & dataType & " WHERE Status='Active' ORDER BY Name ASC")
                ddlDataId.DataTextField = "Name"
                ddlDataId.DataValueField = "Id"
                ddlDataId.DataBind()

                If ddlDataId.Items.Count > 1 Then
                    ddlDataId.Items.Insert(0, New ListItem("", ""))
                End If
            End If
        Catch ex As Exception
            ddlDataId.Items.Clear()
            If Session("RoleName") = "Developer" Then
                MessageError(True, ex.ToString())
            End If
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
