<template>
  <div>
    <h2 class="header-title">交易记录</h2>
    <!-- <el-button style="float:right" type="primary" @click="drawerAdd=true">新增</el-button> -->
    <el-table :data="records" style="width: 100%">
      <el-table-column prop="time" label="时间" width="160"></el-table-column>
      <el-table-column prop="value" label="交易额" width="80"></el-table-column>
      <el-table-column prop="balance" label="余额" width="80"></el-table-column>
      <el-table-column prop="type" label="分类" width="80"></el-table-column>
    </el-table>
  </div>
</template>

<script lang="ts">
import Vue from "vue";
import { withToken, getUrl, showError, formatDateTime } from "../common";
export default Vue.extend({
  name: "Home",
  data() {
    return {
      records: []
    };
  },
  methods: {},
  computed: {},
  components: {},
  mounted: function() {
    this.$nextTick(function() {
      Vue.axios
        .post(getUrl("Transaction", "Records"), withToken({}))
        .then(response => {
          for (const record of response.data.data) {
            const time=record.time as string;
            record.time = formatDateTime(time);
            switch(record.type)
            {
              case 1:
                record.type="停车";
                break;
              case 2:
                record.type="充值";
                break;
              case 3:
                record.type="月租续费";
                break;
            }
          }
          this.records = response.data.data;
        })
        .catch(showError);
    });
  }
});
</script>

<style scoped>
.el-table .cell {
  white-space: pre-line;
  word-wrap: break-word;
}

.cell .el-button {
  margin-right: 6px;
}
</style>
