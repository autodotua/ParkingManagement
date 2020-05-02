<template>
  <div>
    <div slot="header" class="clearfix">
      <span>我的车辆</span>
    </div>
    <el-table :data="cars" style="width: 100%">
      <el-table-column prop="licensePlate" label="车牌" width="120"></el-table-column>
      <el-table-column prop="records" label="停车次数" width="80"></el-table-column>
      <el-table-column fixed="right" label="操作" width="100">
        <template slot-scope="scope">
          <el-button @click="viewDetail(scope.row)" type="text">详情</el-button>
          <el-button @click="deleteCar(scope.row)" type="text">删除</el-button>
        </template>
      </el-table-column>
    </el-table>
    <el-drawer title="停车记录" :visible.sync="drawer" :with-header="true" size="360px">
       <el-table :data="parkRecords" style="width: 100%">
      <el-table-column prop="enterTime" label="进场" width="136"></el-table-column>
      <el-table-column prop="leaveTime" label="离场" width="136s"></el-table-column>
      <el-table-column prop="parkArea.name" label="停车场" width="80"></el-table-column>
       </el-table>
    </el-drawer>
  </div>
</template>

<script lang="ts">
import Vue from "vue";
import Cookies from "js-cookie";
import { withToken ,formatDateTime} from "../common";
export default Vue.extend({
  name: "Home",
  data() {
    return { drawer: false, cars: [], balance: 0, expireTime: "",parkRecords:[] };
  },
  methods: {
    formatDateTime(time: string)
    {
return time;
    },
    viewDetail(row: any) {
      this.drawer = true;
      Vue.axios
        .post(
          "http://localhost:8520/User/Car",
          withToken({ Type: "detail", CarID: row.id  })
        )
        .then(response => {
          for (const record of response.data.data.parkRecords) {
           let date=new Date(record.enterTime);
           record.enterTime=formatDateTime(date);
            date=new Date(record.leaveTime);
           record.leaveTime=formatDateTime(date);
          }
          this.parkRecords=response.data.data.parkRecords;
          console.log(response.data);
        })
        .catch(r => {
          console.log(r.Message);
        });
    },
    deleteCar(row: object) {
      console.log(row);
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
      const userID = Cookies.get("userID") || "";
      Vue.axios
        .post("http://localhost:8520/User/Home", withToken({}))
        .then(response => {
          this.cars = response.data.data.cars;
          this.balance = response.data.data.balance;
          this.expireTime = response.data.data.expireTime;
          console.log(response.data);
        })
        .catch(r => {
          console.log(r.Message);
        });
    });
  }
});
</script>

<style scoped>
.el-table .cell {
  white-space: pre-line;
  word-wrap: break-word;
}
</style>
