Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Database_Table_Edit
    Inherits Page

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Private Property ColumnTable As DataTable
        Get
            If ViewState("ColumnTable") Is Nothing Then
                Dim dt As New DataTable()
                dt.Columns.Add("FieldName")
                dt.Columns.Add("DataType")
                dt.Columns.Add("IsKey")
                dt.Columns.Add("IsNotNull")

                dt.Rows.Add(dt.NewRow())
                ViewState("ColumnTable") = dt
            End If

            Return CType(ViewState("ColumnTable"), DataTable)
        End Get

        Set(value As DataTable)
            ViewState("ColumnTable") = value
        End Set
    End Property

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindData("PriceGroups")
        End If
    End Sub

    Protected Sub gvList_RowDataBound(sender As Object, e As GridViewRowEventArgs)
        MessageError(False, String.Empty)
        Try
            If e.Row.RowType = DataControlRowType.DataRow Then

                Dim row As DataRowView = CType(e.Row.DataItem, DataRowView)

                Dim fieldName As TextBox = TryCast(e.Row.FindControl("txtFieldName"), TextBox)
                Dim type As DropDownList = TryCast(e.Row.FindControl("ddlType"), DropDownList)
                Dim primaryKey As CheckBox = TryCast(e.Row.FindControl("chkKey"), CheckBox)
                Dim notNull As CheckBox = TryCast(e.Row.FindControl("chkNotNull"), CheckBox)

                If fieldName IsNot Nothing AndAlso row("FieldName") IsNot DBNull.Value Then
                    fieldName.Text = row("FieldName").ToString()
                End If

                If type IsNot Nothing AndAlso row("DataType") IsNot DBNull.Value Then
                    Dim val As String = row("DataType").ToString()

                    If type.Items.FindByValue(val) IsNot Nothing Then
                        type.SelectedValue = val
                    End If
                End If

                If primaryKey IsNot Nothing Then
                    Boolean.TryParse(row("IsKey").ToString(), primaryKey.Checked)
                End If

                If notNull IsNot Nothing Then
                    Boolean.TryParse(row("IsNotNull").ToString(), notNull.Checked)
                End If
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub BindData(tableName As String)
        Try

            Dim dt As New DataTable()
            dt.Columns.Add("FieldName")
            dt.Columns.Add("DataType")
            dt.Columns.Add("IsKey", GetType(Boolean))
            dt.Columns.Add("IsNotNull", GetType(Boolean))

            Using con As New SqlConnection(myConn)

                Dim sql As String = "SELECT c.COLUMN_NAME, c.DATA_TYPE, c.CHARACTER_MAXIMUM_LENGTH, c.NUMERIC_PRECISION, c.NUMERIC_SCALE, c.IS_NULLABLE, CASE WHEN k.COLUMN_NAME IS NOT NULL THEN 1 ELSE 0 END AS IsKey FROM INFORMATION_SCHEMA.COLUMNS c LEFT JOIN (SELECT ku.TABLE_NAME, ku.COLUMN_NAME FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE ku ON tc.CONSTRAINT_NAME = ku.CONSTRAINT_NAME WHERE tc.CONSTRAINT_TYPE = 'PRIMARY KEY') k ON c.TABLE_NAME = k.TABLE_NAME AND c.COLUMN_NAME = k.COLUMN_NAME WHERE c.TABLE_NAME = @TableName ORDER BY c.ORDINAL_POSITION"

                Dim cmd As New SqlCommand(sql, con)
                cmd.Parameters.AddWithValue("@TableName", tableName)

                Dim da As New SqlDataAdapter(cmd)
                Dim source As New DataTable()

                da.Fill(source)

                For Each r As DataRow In source.Rows

                    Dim dr As DataRow = dt.NewRow()

                    '========================
                    ' COLUMN NAME
                    '========================
                    dr("FieldName") = r("COLUMN_NAME").ToString()

                    '========================
                    ' FORMAT DATA TYPE
                    '========================
                    Dim type As String = r("DATA_TYPE").ToString()

                    If type = "nvarchar" OrElse type = "varchar" Then

                        Dim len As String = r("CHARACTER_MAXIMUM_LENGTH").ToString()
                        If len = "-1" Then len = "max"

                        type &= "(" & len & ")"

                    ElseIf type = "decimal" OrElse type = "numeric" Then

                        type &= "(" & r("NUMERIC_PRECISION").ToString() & "," &
                            r("NUMERIC_SCALE").ToString() & ")"

                    End If

                    dr("DataType") = type

                    '========================
                    ' PK
                    '========================
                    dr("IsKey") = Convert.ToBoolean(r("IsKey"))

                    '========================
                    ' NOT NULL
                    '========================
                    dr("IsNotNull") = (r("IS_NULLABLE").ToString() = "NO")

                    dt.Rows.Add(dr)

                Next

            End Using

            '========================
            ' SAVE TO VIEWSTATE
            '========================
            ColumnTable = dt

            '========================
            ' BIND TO GRID (EDIT MODE)
            '========================
            BindGrid()

        Catch ex As Exception
            MessageError(True, ex.Message)
        End Try
    End Sub

    Protected Sub BindGrid()
        Try
            gvList.DataSource = ColumnTable
            gvList.DataBind()
        Catch ex As Exception
            MessageError(True, ex.Message)
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
