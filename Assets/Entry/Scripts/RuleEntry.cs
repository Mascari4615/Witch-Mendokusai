using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(RuleEntry), menuName = "Variable/Entry/RuleEntry")]
public class RuleEntry : BaseEntry
{
    public List<EventEntry> TriggeredBy => triggeredBy;
    public List<EventEntry> Triggers => triggers;
    public List<Criteria> Criteria => criteria;
    public List<Modification> Modifications => modifications;
    // public List<> Execute => execute;
    
    [SerializeField] private List<EventEntry> triggeredBy;
    [SerializeField] private List<EventEntry> triggers;
    [SerializeField] private List<Criteria> criteria;
    [SerializeField] private List<Modification> modifications;
    // [SerializeField] private List<> execute;
}

[Serializable]
public class Criteria
{
    public FactEntry FactEntry;
    public ComparisonOperator ComparisonOperator;
    public int Value;
}

public enum ComparisonOperator
{
    Equal,
    NotEqual,
    GreaterThan,
    LessThan,
    GreaterThanOrEqualTo,
    LessThanOrEqualTo
}

[Serializable]
public class Modification
{
    public FactEntry FactEntry;
    public ArithmeticOperator ArithmeticOperator;
    public int Value;
}

public enum ArithmeticOperator
{
    Set,
    Add,
    Subtract,
    Multiply,
    Divide,
    Remainder,
    Power
}