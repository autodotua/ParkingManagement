<template>
  <div>
    <div slot="header" class="clearfix">
      <span>我的车辆</span>
    </div>
    <el-button style="float:right" type="primary" @click="drawerAdd=true">新增</el-button>
    <el-table :data="cars" style="width: 100%">
      <el-table-column prop="licensePlate" label="车牌" width="120"></el-table-column>
      <el-table-column prop="records" label="停车次数" width="80"></el-table-column>
      <el-table-column fixed="right" label="操作" width="100">
        <template slot-scope="scope">
          <el-button @click="viewDetail(scope.row)" type="text">详情</el-button>
          <el-popconfirm title="是否确认删除？" class="delete-popup" @onConfirm="deleteCar(scope.row)">
            <el-button slot="reference" type="text">删除</el-button>
          </el-popconfirm>
        </template>
      </el-table-column>
    </el-table>
    <el-drawer title="停车记录" :visible.sync="drawerDetail" :with-header="true" size="360px">
      <el-table :data="parkRecords" style="width: 100%">
        <el-table-column prop="enterTime" label="进场" width="136"></el-table-column>
        <el-table-column prop="leaveTime" label="离场" width="136s"></el-table-column>
        <el-table-column prop="parkArea.name" label="停车场" width="80"></el-table-column>
      </el-table>
    </el-drawer>
    <el-drawer title="新增车辆" :visible.sync="drawerAdd" :with-header="true" size="160px">
      <el-form label-position="top" style="margin-left:12px">
        <el-form-item label="车牌">
          <el-input v-model="licensePlate" placeholder="浙B12345" style="width:120px"></el-input>
        </el-form-item>
        <el-button type="primary" @click="confirmAdd">确定</el-button>
      </el-form>
    </el-drawer>
  </div>
</template>

<script lang="ts">
import Vue from "vue";
import { withToken, getUrl, showError, formatDateTime } from "../common";
export default Vue.extend({
  name: "Home",
  data() {
    return {
      drawerDetail: false,
      drawerAdd: false,
      cars: [],
      balance: 0,
      expireTime: "",
      parkRecords: [],
      licensePlate: ""
    };
  },
  methods: {
    formatDateTime(time: string) {
      return time;
    },
    viewDetail(row: any) {
      this.drawerDetail = true;
      Vue.axios
        .post(
          getUrl("User", "Car"),
          withToken({ Type: "detail", CarID: row.id })
        )
        .then(response => {
          for (const record of response.data.data.parkRecords) {
            let date = new Date(record.enterTime);
            record.enterTime = formatDateTime(date);
            date = new Date(record.leaveTime);
            record.leaveTime = formatDateTime(date);
          }
          this.parkRecords = response.data.data.parkRecords;
        })
        .catch(showError);
    },
    deleteCar(row: any) {
      Vue.axios
        .post(
          getUrl("User", "Car"),
          withToken({ Type: "delete", CarID: row.id })
        )
        .then(response => {
          location.reload();
        })
        .catch(showError);
    },
    confirmAdd(row: any) {
      if (!this.isLicensePlate(this.licensePlate)) {
        showError("您输入的车牌不正确");
        return;
      }
      Vue.axios
        .post(
          getUrl("User", "Car"),
          withToken({ Type: "add", LicensePlate: this.licensePlate })
        )
        .then(response => {
          location.reload();
        })
        .catch(showError);
    },

    isLicensePlate(str: string) {
      return /^(([京津沪渝冀豫云辽黑湘皖鲁新苏浙赣鄂桂甘晋蒙陕吉闽贵粤青藏川宁琼使领][A-Z](([0-9]{5}[DF])|([DF]([A-HJ-NP-Z0-9])[0-9]{4})))|([京津沪渝冀豫云辽黑湘皖鲁新苏浙赣鄂桂甘晋蒙陕吉闽贵粤青藏川宁琼使领][A-Z][A-HJ-NP-Z0-9]{4}[A-HJ-NP-Z0-9挂学警港澳使领]))$/.test(
        str
      );
    }
  },
  computed: {
    displayBalance(): string {
      return this.balance + "元";
    },
    displayExpireTime(): string {
      if (this.expireTime.startsWith("0001")) {
        return "无";
      }
      return this.expireTime;
    }
  },
  components: {},
  mounted: function() {
    this.$nextTick(function() {
      Vue.axios
        .post("http://localhost:8520/User/Home", withToken({}))
        .then(response => {
          this.cars = response.data.data.cars;
          this.balance = response.data.data.balance;
          this.expireTime = response.data.data.expireTime;
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
