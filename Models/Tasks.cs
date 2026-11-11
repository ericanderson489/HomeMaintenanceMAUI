using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeMaintenanceMAUI.Models 
{
  public class Tasks 
  {
    private string m_name;
    private string m_description;
    private Status m_status;
    private DateTime m_dueDate;
    private Category m_category;
    private Frequency m_frequency;
    public Tasks(string name, string description, DateTime dueDate, Category category, Frequency frequency) 
    {
      m_name = name;
      m_description = description;
      m_dueDate = dueDate;
      m_status = Status.Pending;
      m_category = category;
      m_frequency = frequency;
    }
    public string GetName() { return m_name; }
    public void SetName(string name) { m_name = name; }
    public string GetDescription() { return m_description; }
    public void SetDescription(string description) { m_description = description; }
    public DateTime GetDate() { return m_dueDate; }
    public void SetDate(DateTime date) { m_dueDate = date; }
    public Status GetStatus() { return m_status; }
    public void SetStatus(Status status) { m_status = status; }
    public Category GetCategory() { return m_category; }
    public void SetCategory(Category category) { m_category = category; }
    public Frequency GetFrequency() { return m_frequency; }
    public void SetFrequency(Frequency frequency) { m_frequency = frequency; }
  }
}
