<template>
  <v-card rounded="lg">
    <v-card-title class="pa-5 pb-1">
      Approval Workflow
    </v-card-title>

    <v-card-text class="pa-5 pt-3">
      <p 
        v-if="approvers.length === 0" 
        class="text-body-2 text-medium-emphasis"
      >
        No approval required — preferred vendors under $1,000 are auto-approved.
      </p>

      <div 
        v-else 
        class="d-flex flex-column" 
        style="gap: 8px"
      >
        <v-sheet
          v-for="approver in approvers"
          :key="approver"
          :color="colors[approver]"
          rounded="lg"
          class="pa-3"
        >
          <div 
            class="font-weight-medium"
          >
            {{ approver }}
          </div>
          <div 
            class="text-body-2 text-medium-emphasis"
          >
            {{ descriptions[approver] }}
          </div>
        </v-sheet>
      </div>
    </v-card-text>
  </v-card>
</template>

<script setup lang="ts">
defineProps<{
  approvers: string[]
}>()

const colors: Record<string, string> = {
  Manager: 'blue-lighten-5',
  Director: 'orange-lighten-5',
  VP: 'deep-purple-lighten-5',
}

const descriptions: Record<string, string> = {
  Manager: 'Standard invoices',
  Director: 'Invoices $1,000+',
  VP: 'Invoices $10,000+',
}
</script>
