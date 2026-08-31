Imports System.Data.SqlClient

Partial Class Setting_Validation_Detail_Add
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Private Property ValidationDetails As List(Of ValidationDetailModel)
        Get
            If ViewState("ValidationDetails") Is Nothing Then
                ViewState("ValidationDetails") = New List(Of ValidationDetailModel)
            End If
            Return CType(ViewState("ValidationDetails"), List(Of ValidationDetailModel))
        End Get

        Set(value As List(Of ValidationDetailModel))
            ViewState("ValidationDetails") = value
        End Set
    End Property

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/validation", False)
            Exit Sub
        End If

        If String.IsNullOrEmpty(Request.QueryString("validationid")) Then
            Response.Redirect("~/setting/validation", False)
            Exit Sub
        End If

        lblId.Text = Request.QueryString("validationid").ToString()
        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindValidation()

            ddlValidation.SelectedValue = lblId.Text
            ddlValidation.Enabled = False

            ValidationDetails.Add(New ValidationDetailModel())
            BindRepeater()
        End If
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            SaveRepeater()

            For i As Integer = 0 To ValidationDetails.Count - 1
                Dim item = ValidationDetails(i)
                If String.IsNullOrWhiteSpace(item.FieldName) Then
                    MessageError(True, "FIELD NAME IS REQUIRED. (ROW " & (i + 1).ToString() & ")")
                    Exit Sub
                End If
                If String.IsNullOrWhiteSpace(item.Operator) Then
                    MessageError(True, "OPERATOR IS REQUIRED. (ROW " & (i + 1).ToString() & ")")
                    Exit Sub
                End If
                If String.IsNullOrWhiteSpace(item.GroupNo) Then
                    item.GroupNo = "1"
                End If
            Next

            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()

                For Each item In ValidationDetails
                    Dim thisId As String = settingClass.CreateId("SELECT TOP 1 Id FROM ValidationDetails ORDER BY Id DESC")

                    Using thisCmd As New SqlCommand("INSERT INTO ValidationDetails (Id, ValidationId, GroupNo, FieldName, Operator, CompareValue, DataType) VALUES (@Id, @ValidationId, @GroupNo, @FieldName, @Operator, @CompareValue, @DataType)", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", thisId)
                        thisCmd.Parameters.AddWithValue("@ValidationId", ddlValidation.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@GroupNo", item.GroupNo)
                        thisCmd.Parameters.AddWithValue("@FieldName", item.FieldName)
                        thisCmd.Parameters.AddWithValue("@Operator", item.Operator)
                        thisCmd.Parameters.AddWithValue("@CompareValue", item.CompareValue)
                        thisCmd.Parameters.AddWithValue("@DataType", item.DataType)
                        thisCmd.ExecuteNonQuery()
                    End Using
                Next
            End Using

            Dim url As String = String.Format("~/setting/validation/detail?validationid={0}", ddlValidation.SelectedValue)
            Response.Redirect(url, False)
        Catch ex As Exception
            MessageError(True, ex.Message)
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Dim url As String = String.Format("~/setting/validation/detail?validationid={0}", ddlValidation.SelectedValue)
        Response.Redirect(url, False)
    End Sub

    Protected Sub rptDetail_ItemDataBound(sender As Object, e As RepeaterItemEventArgs)
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim model As ValidationDetailModel = CType(e.Item.DataItem, ValidationDetailModel)

            Dim ddlGroup As DropDownList = CType(e.Item.FindControl("ddlGroupNo"), DropDownList)
            Dim ddlField As DropDownList = CType(e.Item.FindControl("ddlFieldName"), DropDownList)
            Dim ddlOperator As DropDownList = CType(e.Item.FindControl("ddlOperator"), DropDownList)
            Dim ddlType As DropDownList = CType(e.Item.FindControl("ddlDataType"), DropDownList)
            Dim txtCompare As TextBox = CType(e.Item.FindControl("txtCompareValue"), TextBox)

            txtCompare.Text = model.CompareValue

            ' GROUP NO
            If ddlGroup.Items.FindByValue(model.GroupNo) IsNot Nothing Then
                ddlGroup.SelectedValue = model.GroupNo
            End If

            ' FIELD NAME
            ddlField.DataSource = SettingClass.ListValidationFieldName
            ddlField.DataBind()
            ddlField.Items.Insert(0, New ListItem("", ""))
            If Not String.IsNullOrEmpty(model.FieldName) Then
                If ddlField.Items.FindByValue(model.FieldName) IsNot Nothing Then
                    ddlField.SelectedValue = model.FieldName
                End If
            End If

            ' OPERATOR
            ddlOperator.DataSource = SettingClass.ListValidationOperator
            ddlOperator.DataBind()
            ddlOperator.Items.Insert(0, New ListItem("", ""))
            If Not String.IsNullOrEmpty(model.Operator) Then
                If ddlOperator.Items.FindByValue(model.Operator) IsNot Nothing Then
                    ddlOperator.SelectedValue = model.Operator
                End If
            End If

            ' DATA TYPE
            If ddlType.Items.FindByValue(model.DataType) IsNot Nothing Then
                ddlType.SelectedValue = model.DataType
            End If
        End If
    End Sub

    Protected Sub btnAddRow_Click(sender As Object, e As EventArgs)
        SaveRepeater()
        ValidationDetails.Add(New ValidationDetailModel())
        BindRepeater()
    End Sub

    Protected Sub btnDelete_Click(sender As Object, e As EventArgs)
        SaveRepeater()

        Dim btn As LinkButton = CType(sender, LinkButton)
        ValidationDetails.RemoveAt(btn.CommandArgument)
        If ValidationDetails.Count = 0 Then
            ValidationDetails.Add(New ValidationDetailModel())
        End If
        BindRepeater()
    End Sub

    Protected Sub BindRepeater()
        rptDetail.DataSource = ValidationDetails
        rptDetail.DataBind()
    End Sub

    Protected Sub SaveRepeater()
        ValidationDetails.Clear()

        For Each item As RepeaterItem In rptDetail.Items
            Dim model As New ValidationDetailModel

            model.GroupNo = CType(item.FindControl("ddlGroupNo"), DropDownList).SelectedValue
            model.FieldName = CType(item.FindControl("ddlFieldName"), DropDownList).SelectedValue
            model.Operator = CType(item.FindControl("ddlOperator"), DropDownList).SelectedValue
            model.CompareValue = CType(item.FindControl("txtCompareValue"), TextBox).Text
            model.DataType = CType(item.FindControl("ddlDataType"), DropDownList).SelectedValue

            ValidationDetails.Add(model)
        Next
    End Sub

    Protected Sub BindValidation()
        ddlValidation.Items.Clear()
        Try
            ddlValidation.DataSource = settingClass.GetDataTable("SELECT Id, Name FROM Validations")
            ddlValidation.DataTextField = "Name"
            ddlValidation.DataValueField = "Id"
            ddlValidation.DataBind()

            If ddlValidation.Items.Count > 0 Then
                ddlValidation.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            ddlValidation.Items.Clear()
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

<Serializable>
Public Class ValidationDetailModel

    Public Property GroupNo As String
    Public Property FieldName As String
    Public Property [Operator] As String
    Public Property CompareValue As String
    Public Property DataType As String
End Class
