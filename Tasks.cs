using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeMaintenanceMAUI 
{
  public enum Status {
    Pending,
    Complete
  }
  public enum Frequency {
    Weekly,
    Monthly,
    Quarterly,
    Annually
  }

  public enum Category {
    Landscaping,
    Appliances,
    Fixtures,
    Safety,
    Security,
    Cleaning,
    Other
  }
  public class Tasks 
  {
    private string m_name;
    private string m_description;
    private string m_type;
    private Status m_status;
    private DateTime m_dueDate;
    private Category m_category;
    public Tasks(string name, string description, string type, DateTime dueDate, Category category) 
    {
      m_name = name;
      m_description = description;
      m_type = type;
      m_dueDate = dueDate;
      m_status = Status.Pending;
      m_category = category;
    }
    public string GetName() { return m_name; }
    public void SetName(string name) { m_name = name; }
    public string GetDescription() { return m_description; }
    public void SetDescription(string description) { m_description = description; }
    public string GetTaskType() { return m_type; }
    public void SetType(string type) { m_type = type; }
    public DateTime GetDate() { return m_dueDate; }
    public void SetDate(DateTime date) { m_dueDate = date; }
    public Status GetStatus() { return m_status; }
    public void SetStatus(Status status) { m_status = status; }
    public Category GetCategory() { return m_category; }
    public void SetCategory(Category category) { m_category = category; }
  }
}
