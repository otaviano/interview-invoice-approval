CREATE KEYSPACE IF NOT EXISTS invoice_approval
  WITH replication = {'class': 'SimpleStrategy', 'replication_factor': 1};

CREATE TABLE IF NOT EXISTS invoice_approval.approval_records (
    id                  uuid,
    amount              decimal,
    is_preferred_vendor boolean,
    approvers           list<text>,
    created_at          timestamp,
    PRIMARY KEY (id)
);
